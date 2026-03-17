#!/usr/bin/env node
const fs = require("fs");

const playwrightModulePath =
  process.env.PLAYWRIGHT_MODULE_PATH || "/tmp/ahasale-pw/node_modules/playwright";
const { chromium } = require(playwrightModulePath);

const baseUrl = (process.env.BASE_URL || "https://ahasale.local").replace(/\/$/, "");
const adminUser = process.env.ADMIN_USER || "admin";
const adminPass = process.env.ADMIN_PASS || "123456";
const productName = process.env.PRODUCT_NAME || "";
const serviceName = process.env.SERVICE_NAME || "";
const supplyName = process.env.RUNTIME_VATTU_NAME || "";
const invoiceId = process.env.RUNTIME_INVOICE_ID || "";
const importNote = process.env.RUNTIME_IMPORT_NOTE || "";
const invoiceCustomerName = process.env.RUNTIME_INVOICE_CUSTOMER_NAME || "";
const invoiceCustomerPhone = process.env.RUNTIME_INVOICE_PHONE || "";
const includeWarehouse = process.env.RUNTIME_INCLUDE_WAREHOUSE === "1";
const includeInvoiceDetail = process.env.RUNTIME_INCLUDE_INVOICE_DETAIL === "1";
const includeServiceCard = process.env.RUNTIME_INCLUDE_SERVICE_CARD === "1";
const includeStudent = process.env.RUNTIME_INCLUDE_STUDENT === "1";
const includeSupplyImport = process.env.RUNTIME_INCLUDE_SUPPLY_IMPORT === "1";
let activePage = null;
const chromeCandidates = [
  process.env.PLAYWRIGHT_CHROME_PATH || "",
  "/Applications/Google Chrome.app/Contents/MacOS/Google Chrome",
  "/Applications/Chromium.app/Contents/MacOS/Chromium",
];

function requireEnv(value, label) {
  if (!value) {
    throw new Error(`${label} is required.`);
  }
}

async function selectFirstNonEmpty(page, selector) {
  const locator = page.locator(selector).first();
  await locator.waitFor({ state: "attached", timeout: 60000 });
  const value = await locator.evaluate((element) => {
    const options = Array.from(element.options || []);
    const chosen = options.find((item) => item.value);
    return chosen ? chosen.value : "";
  });
  if (!value) {
    throw new Error(`No selectable option for ${selector}`);
  }
  await locator.selectOption(value, { force: true });
  return value;
}

async function fillInput(page, selector, value) {
  const locator = page.locator(selector).first();
  await locator.waitFor({ state: "visible", timeout: 60000 });
  await locator.fill("");
  await locator.fill(String(value));
}

async function setLocatorValue(locator, value) {
  await locator.waitFor({ state: "visible", timeout: 60000 });
  await locator.evaluate((element, nextValue) => {
    element.value = String(nextValue);
    element.dispatchEvent(new Event("input", { bubbles: true }));
    element.dispatchEvent(new Event("change", { bubbles: true }));
  }, String(value));
}

async function setAnyLocatorValue(locator, value) {
  await locator.first().evaluate((element, nextValue) => {
    element.value = String(nextValue);
    element.dispatchEvent(new Event("input", { bubbles: true }));
    element.dispatchEvent(new Event("change", { bubbles: true }));
  }, String(value));
}

async function clickAndWait(page, locator) {
  await locator.click();
  await page.waitForLoadState("networkidle").catch(() => {});
  await page.waitForTimeout(1000);
}

async function gotoWithRetry(page, url, options, maxAttempts = 3) {
  let lastError = null;
  for (let attempt = 1; attempt <= maxAttempts; attempt += 1) {
    try {
      await page.goto(url, options);
      return;
    } catch (error) {
      lastError = error;
      const message = String(error && error.message ? error.message : error);
      const shouldRetry =
        message.includes("ERR_NETWORK_CHANGED") ||
        message.includes("ERR_ABORTED") ||
        message.includes("Navigation timeout");
      if (!shouldRetry || attempt === maxAttempts) {
        throw error;
      }
      await page.waitForTimeout(1000 * attempt);
    }
  }
  throw lastError;
}

function invoiceTableRows(page) {
  return page.locator("#table-main tbody tr");
}

function serviceCardRows(page) {
  return page.locator("#thedichvu table tbody tr");
}

function paymentHistoryRows(page) {
  return page.locator("div[data-title-caption='Thanh toán'] table tbody tr");
}

async function waitForInvoiceRow(page, text, beforeCount) {
  await page.waitForFunction(
    ([selector, expected, minCount]) => {
      const rows = Array.from(document.querySelectorAll(selector));
      const visibleRows = rows.filter((row) => {
        if (!(row instanceof HTMLElement)) {
          return false;
        }
        return row.offsetParent !== null;
      });
      if (visibleRows.length <= minCount) {
        return false;
      }
      return visibleRows.some((row) => row.innerText.includes(expected));
    },
    ["#table-main tbody tr", text, beforeCount],
    { timeout: 60000 },
  );
}

async function getVisibleRow(page, selector, text) {
  await page.waitForFunction(
    ([rowSelector, expected]) => {
      const rows = Array.from(document.querySelectorAll(rowSelector));
      return rows.some((row) => {
        if (!(row instanceof HTMLElement)) {
          return false;
        }
        return row.offsetParent !== null && row.innerText.includes(expected);
      });
    },
    [selector, text],
    { timeout: 60000 },
  );

  const rowIndex = await page.locator(selector).evaluateAll((rows, expected) => {
    return rows.findIndex((row) => {
      if (!(row instanceof HTMLElement)) {
        return false;
      }
      return row.offsetParent !== null && row.innerText.includes(expected);
    });
  }, text);

  if (rowIndex < 0) {
    throw new Error(`Cannot find visible row for ${text} in ${selector}`);
  }
  return page.locator(selector).nth(rowIndex);
}

async function getFirstVisibleRow(page, selector) {
  await page.waitForFunction(
    (rowSelector) => {
      const rows = Array.from(document.querySelectorAll(rowSelector));
      return rows.some((row) => row instanceof HTMLElement && row.offsetParent !== null);
    },
    selector,
    { timeout: 60000 },
  );

  const rowIndex = await page.locator(selector).evaluateAll((rows) => {
    return rows.findIndex((row) => row instanceof HTMLElement && row.offsetParent !== null);
  });

  if (rowIndex < 0) {
    throw new Error(`Cannot find any visible row in ${selector}`);
  }
  return page.locator(selector).nth(rowIndex);
}

function parseCurrencyValue(value) {
  const digits = String(value || "").replace(/[^\d]/g, "");
  return digits ? parseInt(digits, 10) : 0;
}

async function submitPaymentForm(page, submitSelector, urlFragment) {
  const paymentRequestPromise = page
    .waitForRequest(
      (request) =>
        request.method() === "POST" && request.url().toLowerCase().includes(urlFragment.toLowerCase()),
      { timeout: 60000 },
    )
    .catch(() => null);
  const paymentButton = page.locator(submitSelector).first();
  await paymentButton.evaluate((element) => {
    const form = element.form;
    if (!form) {
      throw new Error("Payment button is not attached to a form.");
    }
    const hidden = document.createElement("input");
    hidden.type = "hidden";
    hidden.name = element.name;
    hidden.value = element.value || "THANH TOÁN";
    form.appendChild(hidden);
    form.submit();
  });
  const paymentRequest = await paymentRequestPromise;
  if (!paymentRequest) {
    throw new Error(`Payment form ${submitSelector} did not submit a POST request.`);
  }
  await page.waitForLoadState("networkidle").catch(() => {});
  await page.waitForTimeout(2000);
}

async function payInvoice(page) {
  await page.evaluate(() => {
    const modal = document.getElementById("form_thanhtoan");
    if (modal) {
      modal.style.display = "block";
    }
  });

  const amountInput = page.locator("input[id$='txt_sotien_thanhtoan_congno']").first();
  await amountInput.waitFor({ state: "visible", timeout: 60000 });

  const amountValue = await amountInput.inputValue();
  const paymentAmount = parseCurrencyValue(amountValue);
  if (!paymentAmount) {
    throw new Error(`Invoice payment amount is empty or zero: ${amountValue}`);
  }
  console.log(`Playwright: paying invoice ${invoiceId} amount=${paymentAmount}`);

  await page
    .locator("select[id$='ddl_hinhthuc_thanhtoan']")
    .first()
    .selectOption({ label: "Tiền mặt" }, { force: true });
  await fillInput(page, "input[id$='txt_sotien_thanhtoan_congno']", String(paymentAmount));

  await submitPaymentForm(page, "input[id$='but_thanhtoan']", "/gianhang/admin/quan-ly-hoa-don/chi-tiet.aspx");
  await gotoWithRetry(page, `${baseUrl}/gianhang/admin/quan-ly-hoa-don/chi-tiet.aspx?id=${invoiceId}`, {
    waitUntil: "domcontentloaded",
  });
  await page.waitForLoadState("networkidle");

  const bodyText = await page.locator("body").innerText();
  if (!bodyText.includes("Đã thanh toán")) {
    const paymentLines = (await paymentHistoryRows(page).allInnerTexts().catch(() => []))
      .map((item) => item.trim())
      .filter(Boolean)
      .slice(0, 5)
      .join(" | ");
    const statusLine =
      bodyText
        .split("\n")
        .map((item) => item.trim())
        .find((item) => item.includes("thanh toán") || item.includes("Thanh toán")) || "";
    throw new Error(
      `Invoice detail did not show paid status after payment. status="${statusLine}" history="${paymentLines}"`,
    );
  }
  if (!bodyText.includes("Tiền mặt")) {
    throw new Error("Invoice payment history did not record Tiền mặt.");
  }
  if (await paymentHistoryRows(page).count() === 0) {
    throw new Error("Invoice payment history is empty after payment.");
  }

  await gotoWithRetry(page, `${baseUrl}/gianhang/admin/quan-ly-thu-chi/Default.aspx`, {
    waitUntil: "domcontentloaded",
  });
  await page.waitForLoadState("networkidle");
  const thuChiText = await page.locator("body").innerText();
  if (!thuChiText.includes(`HĐ #${invoiceId}`)) {
    throw new Error(`Thu chi did not show the auto receipt for invoice ${invoiceId}.`);
  }
  if (!thuChiText.includes("Thu tự động từ hóa đơn")) {
    throw new Error("Thu chi did not show the auto receipt group.");
  }
}

async function payServiceCard(page, card) {
  await gotoWithRetry(page, `${baseUrl}/gianhang/admin/quan-ly-the-dich-vu/chi-tiet.aspx?id=${card.cardId}`, {
    waitUntil: "domcontentloaded",
  });
  await page.waitForLoadState("networkidle");
  await page.evaluate(() => {
    const modal = document.getElementById("form_thanhtoan");
    if (modal) {
      modal.style.display = "block";
    }
  });

  const amountInput = page.locator("input[id$='txt_sotien_thanhtoan_congno']").first();
  await amountInput.waitFor({ state: "visible", timeout: 60000 });
  const paymentAmount = parseCurrencyValue(await amountInput.inputValue());
  if (!paymentAmount) {
    throw new Error(`Service-card payment amount is empty for card ${card.cardId}`);
  }

  await page
    .locator("select[id$='ddl_hinhthuc_thanhtoan']")
    .first()
    .selectOption({ label: "Tiền mặt" }, { force: true });
  await fillInput(page, "input[id$='txt_sotien_thanhtoan_congno']", String(paymentAmount));
  await submitPaymentForm(page, "input[id$='but_thanhtoan']", "/gianhang/admin/quan-ly-the-dich-vu/chi-tiet.aspx");
  await gotoWithRetry(page, `${baseUrl}/gianhang/admin/quan-ly-the-dich-vu/chi-tiet.aspx?id=${card.cardId}`, {
    waitUntil: "domcontentloaded",
  });
  await page.waitForLoadState("networkidle");
  await page
    .waitForFunction(() => document.documentElement && document.documentElement.innerHTML.includes("Đã thanh toán"), {
      timeout: 60000,
    })
    .catch(() => {});

  const pageHtml = await page.content();
  if (!pageHtml.includes("Đã thanh toán")) {
    const stamp = Date.now();
    fs.writeFileSync(`/tmp/gianhang-admin-service-card-${stamp}.html`, pageHtml, "utf8");
    const historyText = await page.locator("table tbody tr").allInnerTexts().catch(() => []);
    throw new Error(
      `Service card ${card.cardId} did not show paid status. history="${historyText
        .map((item) => item.trim())
        .filter(Boolean)
        .slice(0, 8)
        .join(" | ")}" dump=/tmp/gianhang-admin-service-card-${stamp}.html`,
    );
  }

  await gotoWithRetry(page, `${baseUrl}/gianhang/admin/quan-ly-thu-chi/Default.aspx`, {
    waitUntil: "domcontentloaded",
  });
  await page.waitForLoadState("networkidle");
  const thuChiText = await page.locator("body").innerText();
  if (!thuChiText.includes(`#${card.cardId}`)) {
    throw new Error(`Thu chi did not show the auto receipt for service card ${card.cardId}.`);
  }
  if (!thuChiText.includes("Thu tự động từ thẻ dịch vụ")) {
    throw new Error("Thu chi did not show the service-card auto receipt group.");
  }
}

async function payWarehouseOrder(page, orderId) {
  await gotoWithRetry(page, `${baseUrl}/gianhang/admin/quan-ly-kho-hang/chi-tiet-nhap-hang.aspx?id=${orderId}`, {
    waitUntil: "domcontentloaded",
  });
  await page.waitForLoadState("networkidle");
  await page.evaluate(() => {
    const modal = document.getElementById("form_thanhtoan");
    if (modal) {
      modal.style.display = "block";
    }
  });

  const amountInput = page.locator("input[id$='txt_sotien_thanhtoan_congno']").first();
  await amountInput.waitFor({ state: "visible", timeout: 60000 });
  const paymentAmount = parseCurrencyValue(await amountInput.inputValue());
  if (!paymentAmount) {
    throw new Error(`Warehouse payment amount is empty for order ${orderId}`);
  }

  await page
    .locator("select[id$='ddl_hinhthuc_thanhtoan']")
    .first()
    .selectOption({ label: "Tiền mặt" }, { force: true });
  await fillInput(page, "input[id$='txt_sotien_thanhtoan_congno']", String(paymentAmount));
  await submitPaymentForm(page, "input[id$='but_thanhtoan']", "/gianhang/admin/quan-ly-kho-hang/chi-tiet-nhap-hang.aspx");
  await gotoWithRetry(page, `${baseUrl}/gianhang/admin/quan-ly-kho-hang/chi-tiet-nhap-hang.aspx?id=${orderId}`, {
    waitUntil: "domcontentloaded",
  });
  await page.waitForLoadState("networkidle");

  const bodyText = await page.locator("body").innerText();
  if (!bodyText.includes("Đã thanh toán")) {
    throw new Error(`Warehouse order ${orderId} did not show paid status.`);
  }

  await gotoWithRetry(page, `${baseUrl}/gianhang/admin/quan-ly-thu-chi/Default.aspx`, {
    waitUntil: "domcontentloaded",
  });
  await page.waitForLoadState("networkidle");
  const thuChiText = await page.locator("body").innerText();
  if (!thuChiText.includes(`#${orderId}`)) {
    const stamp = Date.now();
    const rowTexts = await page.locator("#table-main tbody tr").allInnerTexts().catch(() => []);
    fs.writeFileSync(`/tmp/gianhang-admin-thuchi-warehouse-${stamp}.html`, await page.content(), "utf8");
    throw new Error(
      `Thu chi did not show the auto expense for warehouse order ${orderId}. rows="${rowTexts
        .map((item) => item.trim())
        .filter(Boolean)
        .slice(0, 8)
        .join(" | ")}" dump=/tmp/gianhang-admin-thuchi-warehouse-${stamp}.html`,
    );
  }
  if (!thuChiText.includes("Chi tự động nhập hàng")) {
    throw new Error("Thu chi did not show the warehouse auto expense group.");
  }
}

async function importSupplyOrder(page) {
  await gotoWithRetry(page, `${baseUrl}/gianhang/admin/quan-ly-vat-tu/nhap-vat-tu.aspx`, {
    waitUntil: "domcontentloaded",
  });
  await page.waitForLoadState("networkidle");

  const searchInput = page.locator("input[id$='txt_search']").first();
  if (await searchInput.count()) {
    await searchInput.evaluate((element, nextValue) => {
      element.value = String(nextValue);
      element.dispatchEvent(new Event("input", { bubbles: true }));
      element.dispatchEvent(new Event("change", { bubbles: true }));
      if (typeof window.__doPostBack === "function" && element.name) {
        window.__doPostBack(element.name, "");
      }
    }, supplyName);
    await page.waitForLoadState("networkidle").catch(() => {});
    await page.waitForTimeout(1000);
  }

  let row = page.locator("#table-main tbody tr").filter({ hasText: supplyName }).first();
  if ((await row.count()) === 0) {
    row = page.locator("#table-main tbody tr").first();
  }

  const lotCode = `VTRT${Date.now().toString().slice(-8)}`;
  await setAnyLocatorValue(row.locator("input[name^='dvt_']"), "cai");
  await setAnyLocatorValue(row.locator("input[name^='solo_']"), lotCode);
  await setAnyLocatorValue(row.locator("input[name^='giavon_']"), "120000");
  await setAnyLocatorValue(row.locator("input[name^='slsp_']"), "2");
  await setAnyLocatorValue(row.locator("input[name^='ck_']"), "0");
  await setAnyLocatorValue(row.locator("input[name^='nsx_']"), new Date().toLocaleDateString("en-GB"));
  const nextYear = new Date();
  nextYear.setFullYear(nextYear.getFullYear() + 1);
  await setAnyLocatorValue(row.locator("input[name^='hsd_']"), nextYear.toLocaleDateString("en-GB"));

  const addRequestPromise = page
    .waitForRequest(
      (request) =>
        request.method() === "POST" &&
        request.url().toLowerCase().includes("/gianhang/admin/quan-ly-vat-tu/nhap-vat-tu.aspx"),
      { timeout: 60000 },
    )
    .catch(() => null);
  await page.locator("input[id$='but_themvaogio']").first().evaluate((element) => {
    element.click();
  });
  await addRequestPromise;
  await page.waitForLoadState("networkidle").catch(() => {});
  await page.waitForTimeout(1000);
  const supplyLotLocator = page.locator(`input[name^='solo_gh_'][value='${lotCode}']`).first();
  try {
    await supplyLotLocator.waitFor({
      state: "visible",
      timeout: 60000,
    });
  } catch (error) {
    const stamp = Date.now();
    const rowTexts = await page.locator("#_donnhaphang tbody tr").allInnerTexts().catch(() => []);
    const lotValues = await page.locator("input[name^='solo_gh_']").evaluateAll((nodes) =>
      nodes.map((node) => `${node.getAttribute("name") || ""}=${node.value || ""}`),
    ).catch(() => []);
    fs.writeFileSync(`/tmp/gianhang-admin-supply-import-${stamp}.html`, await page.content(), "utf8");
    throw new Error(
      `Supply import cart row did not appear for lot ${lotCode}. lots="${lotValues.join(" | ")}" rows="${rowTexts
        .map((item) => item.trim())
        .filter(Boolean)
        .slice(0, 8)
        .join(" | ")}" dump=/tmp/gianhang-admin-supply-import-${stamp}.html`,
    );
  }

  const supplierSelect = page.locator("select[id$='DropDownList3']").first();
  await supplierSelect.waitFor({ state: "attached", timeout: 60000 });
  const supplierValue = await supplierSelect.evaluate((element) => {
    const options = Array.from(element.options || []);
    const chosen = options.find((item) => item.value);
    return chosen ? chosen.value : "";
  });
  if (supplierValue) {
    await supplierSelect.selectOption(supplierValue, { force: true });
  }
  await fillInput(page, "input[id$='txt_ghichu']", `Runtime import ${supplyName}`);

  const confirmImportButton = page.locator("input[id$='Button1']").first();
  await confirmImportButton.evaluate((element) => {
    element.setAttribute("data-admin-confirm-approved", "1");
  });
  try {
    await Promise.all([
      page.waitForURL(/\/gianhang\/admin\/quan-ly-vat-tu\/vat-tu-da-nhap\.aspx/i, {
        timeout: 60000,
      }),
      confirmImportButton.click({ force: true }),
    ]);
  } catch (error) {
    const stamp = Date.now();
    const bodyText = await page.locator("body").innerText().catch(() => "");
    fs.writeFileSync(`/tmp/gianhang-admin-supply-confirm-${stamp}.html`, await page.content(), "utf8");
    throw new Error(
      `Supply import confirmation did not navigate away. url="${page.url()}" body="${bodyText
        .replace(/\s+/g, " ")
        .slice(0, 600)}" dump=/tmp/gianhang-admin-supply-confirm-${stamp}.html`,
    );
  }
  await page.waitForLoadState("networkidle");

  const orderLink = page
    .locator("#table-main tbody tr a[href*='chi-tiet-nhap-hang.aspx?id=']")
    .first();
  await orderLink.waitFor({ state: "visible", timeout: 60000 });
  const orderHref = (await orderLink.getAttribute("href")) || "";
  const orderIdMatch = orderHref.match(/id=(\d+)/i);
  if (!orderIdMatch) {
    throw new Error("Cannot extract supply import order id after import.");
  }
  return { orderId: orderIdMatch[1] };
}

async function paySupplyOrder(page, orderId) {
  await gotoWithRetry(page, `${baseUrl}/gianhang/admin/quan-ly-vat-tu/chi-tiet-nhap-hang.aspx?id=${orderId}`, {
    waitUntil: "domcontentloaded",
  });
  await page.waitForLoadState("networkidle");
  await page.evaluate(() => {
    const modal = document.getElementById("form_thanhtoan");
    if (modal) {
      modal.style.display = "block";
    }
  });

  const amountInput = page.locator("input[id$='txt_sotien_thanhtoan_congno']").first();
  await amountInput.waitFor({ state: "visible", timeout: 60000 });
  const paymentAmount = parseCurrencyValue(await amountInput.inputValue());
  if (!paymentAmount) {
    throw new Error(`Supply payment amount is empty for order ${orderId}`);
  }

  await page
    .locator("select[id$='ddl_hinhthuc_thanhtoan']")
    .first()
    .selectOption({ label: "Tiền mặt" }, { force: true });
  await fillInput(page, "input[id$='txt_sotien_thanhtoan_congno']", String(paymentAmount));
  await submitPaymentForm(page, "input[id$='but_thanhtoan']", "/gianhang/admin/quan-ly-vat-tu/chi-tiet-nhap-hang.aspx");
  await gotoWithRetry(page, `${baseUrl}/gianhang/admin/quan-ly-vat-tu/chi-tiet-nhap-hang.aspx?id=${orderId}`, {
    waitUntil: "domcontentloaded",
  });
  await page.waitForLoadState("networkidle");

  const pageHtml = await page.content();
  if (!pageHtml.includes("Đã thanh toán")) {
    throw new Error(`Supply order ${orderId} did not show paid status.`);
  }

  await gotoWithRetry(page, `${baseUrl}/gianhang/admin/quan-ly-thu-chi/Default.aspx`, {
    waitUntil: "domcontentloaded",
  });
  await page.waitForLoadState("networkidle");
  const thuChiText = await page.locator("body").innerText();
  if (!thuChiText.includes(`#${orderId}`)) {
    throw new Error(`Thu chi did not show the auto expense for supply order ${orderId}.`);
  }
  if (!thuChiText.includes("Chi tự động nhập vật tư")) {
    throw new Error("Thu chi did not show the supply auto expense group.");
  }
}

async function createStudent(page) {
  const studentName = `Runtime Student ${Date.now().toString().slice(-6)}`;
  const studentPhone = `07${Date.now().toString().slice(-8)}`;

  await gotoWithRetry(page, `${baseUrl}/gianhang/admin/quan-ly-hoc-vien/add.aspx`, {
    waitUntil: "domcontentloaded",
  });
  await page.waitForLoadState("networkidle");

  await fillInput(page, "input[id$='txt_hoten']", studentName);
  await fillInput(page, "input[id$='txt_dienthoai']", studentPhone);
  await selectFirstNonEmpty(page, "select[id$='DropDownList5']");
  await selectFirstNonEmpty(page, "select[id$='DropDownList2']");
  await selectFirstNonEmpty(page, "select[id$='DropDownList1']");
  await fillInput(page, "input[id$='txt_hocphi']", "700000");

  await Promise.all([
    page.waitForURL(/\/gianhang\/admin\/quan-ly-hoc-vien\/Default\.aspx/i, { timeout: 60000 }),
    page.locator("input[id$='button1']").first().click(),
  ]);
  await page.waitForLoadState("networkidle");

  const row = page.locator("tbody tr").filter({ hasText: studentName }).first();
  await row.waitFor({ state: "visible", timeout: 60000 });
  const editLink = row.locator("a[href*='edit.aspx?id=']").first();
  const editHref = (await editLink.getAttribute("href")) || "";
  const studentIdMatch = editHref.match(/id=(\d+)/i);
  if (!studentIdMatch) {
    throw new Error(`Cannot extract student id for ${studentName}`);
  }
  return { studentId: studentIdMatch[1], studentName, studentPhone };
}

async function payStudent(page, student) {
  await gotoWithRetry(page, `${baseUrl}/gianhang/admin/quan-ly-hoc-vien/edit.aspx?id=${student.studentId}`, {
    waitUntil: "domcontentloaded",
  });
  await page.waitForLoadState("networkidle");
  await page.evaluate(() => {
    const modal = document.getElementById("form_thanhtoan");
    if (modal) {
      modal.style.display = "block";
    }
  });

  const amountInput = page.locator("input[id$='txt_sotien_thanhtoan_congno']").first();
  await amountInput.waitFor({ state: "visible", timeout: 60000 });
  const paymentAmount = parseCurrencyValue(await amountInput.inputValue());
  if (!paymentAmount) {
    throw new Error(`Student payment amount is empty for student ${student.studentId}`);
  }

  await page
    .locator("select[id$='ddl_hinhthuc_thanhtoan']")
    .first()
    .selectOption({ label: "Tiền mặt" }, { force: true });
  await fillInput(page, "input[id$='txt_sotien_thanhtoan_congno']", String(paymentAmount));
  await submitPaymentForm(page, "input[id$='but_thanhtoan']", "/gianhang/admin/quan-ly-hoc-vien/edit.aspx");
  await gotoWithRetry(page, `${baseUrl}/gianhang/admin/quan-ly-hoc-vien/edit.aspx?id=${student.studentId}`, {
    waitUntil: "domcontentloaded",
  });
  await page.waitForLoadState("networkidle");

  const pageHtml = await page.content();
  if (!pageHtml.includes("Đã thanh toán")) {
    throw new Error(`Student ${student.studentId} did not show paid status.`);
  }

  await gotoWithRetry(page, `${baseUrl}/gianhang/admin/quan-ly-thu-chi/Default.aspx`, {
    waitUntil: "domcontentloaded",
  });
  await page.waitForLoadState("networkidle");
  const thuChiText = await page.locator("body").innerText();
  if (!thuChiText.includes(`#${student.studentId}`)) {
    throw new Error(`Thu chi did not show the auto receipt for student ${student.studentId}.`);
  }
  if (!thuChiText.includes("Thu tự động từ học viên")) {
    throw new Error("Thu chi did not show the student auto receipt group.");
  }
}

async function login(page) {
  await gotoWithRetry(page, `${baseUrl}/gianhang/admin/login.aspx`, { waitUntil: "domcontentloaded" });
  await page.waitForLoadState("networkidle");
  await selectFirstNonEmpty(page, "select[id$='ddl_chinhanh']");
  await fillInput(page, "input[id$='txt_user']", adminUser);
  await fillInput(page, "input[id$='txt_pass']", adminPass);
  await Promise.all([
    page.waitForURL((url) => !url.pathname.endsWith("/gianhang/admin/login.aspx"), {
      timeout: 60000,
    }),
    page.locator("input[id$='but_login']").click(),
  ]);
  await page.waitForLoadState("networkidle");
}

async function importWarehouse(page) {
  await gotoWithRetry(page, `${baseUrl}/gianhang/admin/quan-ly-kho-hang/nhap-hang.aspx?q=nh`, {
    waitUntil: "domcontentloaded",
  });
  await page.waitForLoadState("networkidle");

  const row = page.locator("tbody tr").filter({ hasText: productName }).first();
  await row.waitFor({ state: "visible", timeout: 60000 });

  const lotCode = `RTPW${Date.now().toString().slice(-8)}`;
  await row.locator("input[name^='dvt_']").fill("chai");
  await row.locator("input[name^='solo_']").fill(lotCode);
  await row.locator("input[name^='giavon_']").fill("200000");
  await row.locator("input[name^='slsp_']").fill("3");
  await row.locator("input[name^='ck_']").fill("0");
  await setLocatorValue(row.locator("input[name^='nsx_']").first(), new Date().toLocaleDateString("en-GB"));
  const nextYear = new Date();
  nextYear.setFullYear(nextYear.getFullYear() + 1);
  await setLocatorValue(row.locator("input[name^='hsd_']").first(), nextYear.toLocaleDateString("en-GB"));

  const addRequestPromise = page
    .waitForRequest(
      (request) =>
        request.method() === "POST" &&
        request.url().toLowerCase().includes("/gianhang/admin/quan-ly-kho-hang/nhap-hang.aspx"),
      { timeout: 60000 },
    )
    .catch(() => null);
  await page.locator("input[id$='but_themvaogio']").first().evaluate((element) => {
    element.click();
  });
  await page.waitForLoadState("networkidle").catch(() => {});
  await page.waitForTimeout(1000);
  const addRequest = await addRequestPromise;
  if (addRequest) {
    fs.writeFileSync(`/tmp/gianhang-admin-import-request-${Date.now()}.txt`, addRequest.postData() || "", "utf8");
  }

  try {
    await page.locator(`input[name^='solo_gh_'][value='${lotCode}']`).first().waitFor({
      state: "visible",
      timeout: 60000,
    });
  } catch (error) {
    const stamp = Date.now();
    fs.writeFileSync(`/tmp/gianhang-admin-import-${stamp}.html`, await page.content(), "utf8");
    throw error;
  }
  const supplierSelect = page.locator("select[id$='DropDownList3']").first();
  await supplierSelect.waitFor({ state: "attached", timeout: 60000 });
  const supplierValue = await supplierSelect.evaluate((element) => {
    const options = Array.from(element.options || []);
    const chosen = options.find((item) => item.value);
    return chosen ? chosen.value : "";
  });
  if (supplierValue) {
    await supplierSelect.selectOption(supplierValue, { force: true });
  }
  await fillInput(page, "input[id$='txt_ghichu']", importNote);

  let confirmRequestPromise = null;
  try {
    const submitButton = page.locator("input[id$='Button1']").first();
    confirmRequestPromise = page
      .waitForRequest(
        (request) =>
          request.method() === "POST" &&
          request.url().toLowerCase().includes("/gianhang/admin/quan-ly-kho-hang/nhap-hang.aspx"),
        { timeout: 60000 },
      )
      .catch(() => null);
    await submitButton.evaluate((element) => {
      element.setAttribute("data-admin-confirm-approved", "1");
      element.click();
    });
    await page.waitForURL(/\/gianhang\/admin\/quan-ly-kho-hang\/don-nhap-hang\.aspx/i, {
      timeout: 60000,
    });

    const confirmRequest = await confirmRequestPromise;
    if (confirmRequest) {
      fs.writeFileSync(`/tmp/gianhang-admin-confirm-request-${Date.now()}.txt`, confirmRequest.postData() || "", "utf8");
    }
  } catch (error) {
    const stamp = Date.now();
    fs.writeFileSync(`/tmp/gianhang-admin-confirm-${stamp}.html`, await page.content(), "utf8");
    if (confirmRequestPromise) {
      const confirmRequest = await confirmRequestPromise.catch(() => null);
      if (confirmRequest) {
        fs.writeFileSync(`/tmp/gianhang-admin-confirm-request-${stamp}.txt`, confirmRequest.postData() || "", "utf8");
      }
    }
    throw error;
  }
  await page.waitForLoadState("networkidle");
  await page.locator("body").waitFor({ state: "visible", timeout: 60000 });

  const orderLink = page
    .locator("#table-main tbody tr a[href*='chi-tiet-nhap-hang.aspx?id=']")
    .first();
  await orderLink.waitFor({ state: "visible", timeout: 60000 });
  const orderHref = (await orderLink.getAttribute("href")) || "";
  const orderIdMatch = orderHref.match(/id=(\d+)/i);
  if (!orderIdMatch) {
    throw new Error("Cannot extract warehouse order id after import.");
  }
  return { orderId: orderIdMatch[1] };
}

async function addInvoiceService(page) {
  const beforeCount = await invoiceTableRows(page).count();
  await fillInput(page, "input[id$='txt_tendichvu']", serviceName);
  await fillInput(page, "input[id$='txt_gia']", "250000");
  await fillInput(page, "input[id$='txt_soluong']", "1");
  await fillInput(page, "input[id$='txt_chietkhau']", "0");
  await selectFirstNonEmpty(page, "select[id$='ddl_nhanvien_chotsale']");
  await fillInput(page, "input[id$='txt_chietkhau_chotsale']", "0");
  await selectFirstNonEmpty(page, "select[id$='ddl_nhanvien_lamdichvu']");
  await fillInput(page, "input[id$='txt_chietkhau_lamdichvu']", "0");
  await fillInput(page, "input[id$='txt_danhgia_dichvu']", "Browser runtime service flow");

  const ratingInput = page.locator("input[name='danhgia_5sao_nhanvien_dv']").first();
  if (await ratingInput.count()) {
    await ratingInput.evaluate((element) => {
      element.value = "5";
      element.dispatchEvent(new Event("change", { bubbles: true }));
    });
  }

  await clickAndWait(page, page.locator("input[id$='but_form_themdichvu']").first());
  await waitForInvoiceRow(page, serviceName, beforeCount);
}

async function addInvoiceProduct(page) {
  const beforeCount = await invoiceTableRows(page).count();
  await page.locator("ul[data-role='tabs'] a[href='#themsanpham']").first().click();
  await page.waitForTimeout(500);

  await fillInput(page, "input[id$='txt_tensanpham']", productName);
  await fillInput(page, "input[id$='txt_gia_sanpham']", "350000");
  await fillInput(page, "input[id$='txt_soluong_sanpham']", "1");
  await fillInput(page, "input[id$='txt_chietkhau_sanpham']", "0");
  await selectFirstNonEmpty(page, "select[id$='ddl_nhanvien_chotsale_sanpham']");
  await fillInput(page, "input[id$='txt_chietkhau_chotsale_sanpham']", "0");

  await clickAndWait(page, page.locator("input[id$='but_form_themsanpham']").first());
  await waitForInvoiceRow(page, productName, beforeCount);
}

async function sellServiceCard(page) {
  const cardName = `Runtime Card ${Date.now().toString().slice(-6)}`;
  await gotoWithRetry(page, 
    `${baseUrl}/gianhang/admin/quan-ly-the-dich-vu/Default.aspx?q=add&sdt=${encodeURIComponent(invoiceCustomerPhone)}&tenkh=${encodeURIComponent(invoiceCustomerName)}`,
    { waitUntil: "domcontentloaded" },
  );
  await page.waitForLoadState("networkidle");

  await selectFirstNonEmpty(page, "select[id$='DropDownList3']");
  await fillInput(page, "input[id$='txt_tenkhachhang']", invoiceCustomerName);
  await fillInput(page, "input[id$='txt_sdt']", invoiceCustomerPhone);
  await fillInput(page, "input[id$='txt_diachi']", "Runtime service-card address");
  await fillInput(page, "input[id$='txt_ghichu']", `Runtime card for invoice ${invoiceId}`);
  await fillInput(page, "input[id$='txt_tenthe']", cardName);
  await page.locator("select[id$='DropDownList1']").first().selectOption({ label: serviceName }).catch(async () => {
    const serviceSelect = page.locator("select[id$='DropDownList1']").first();
    await serviceSelect.waitFor({ state: "attached", timeout: 60000 });
    const serviceValue = await serviceSelect.evaluate((element, name) => {
      const options = Array.from(element.options || []);
      const chosen = options.find((item) => (item.textContent || "").trim() === name);
      return chosen ? chosen.value : "";
    }, serviceName);
    if (!serviceValue) {
      throw new Error(`Cannot select service-card service ${serviceName}`);
    }
    await serviceSelect.selectOption(serviceValue, { force: true });
  });
  await fillInput(page, "input[id$='txt_sobuoi']", "5");
  await fillInput(page, "input[id$='txt_giatrithe']", "500000");
  await fillInput(page, "input[id$='txt_chietkhau_giatrithe']", "0");
  await fillInput(page, "input[id$='txt_chietkhau_chotsale']", "0");
  await selectFirstNonEmpty(page, "select[id$='ddl_nhanvien_chotsale']");

  await Promise.all([
    page.waitForURL(/\/gianhang\/admin\/quan-ly-the-dich-vu\/Default\.aspx/i, { timeout: 60000 }),
    page.locator("input[id$='Button3']").first().click(),
  ]);
  await page.waitForLoadState("networkidle");
  const row = page.locator("tbody tr").filter({ hasText: cardName }).first();
  await row.waitFor({
    state: "visible",
    timeout: 60000,
  });
  const cardLink = row.locator("a[href*='chi-tiet.aspx?id=']").first();
  const cardHref = (await cardLink.getAttribute("href")) || "";
  const cardIdMatch = cardHref.match(/id=(\d+)/i);
  if (!cardIdMatch) {
    throw new Error(`Cannot extract service-card id for ${cardName}`);
  }
  return { cardId: cardIdMatch[1], cardName };
}

async function useServiceCardOnInvoice(page, card) {
  const beforeCount = await invoiceTableRows(page).count();
  await page.locator("ul[data-role='tabs'] a[href='#thedichvu']").first().click();
  await page.waitForTimeout(500);

  const row = serviceCardRows(page).filter({ hasText: card.cardName }).first();
  await row.waitFor({ state: "visible", timeout: 60000 });

  const checkbox = row.locator("input[name^='check_thedv_']").first();
  await checkbox.check({ force: true }).catch(async () => {
    await checkbox.evaluate((element) => {
      element.checked = true;
      element.dispatchEvent(new Event("input", { bubbles: true }));
      element.dispatchEvent(new Event("change", { bubbles: true }));
    });
  });

  await selectFirstNonEmpty(page, "select[id$='ddl_nhanvien_lamdichvu_thedv']");
  await fillInput(page, "input[id$='txt_chietkhau_lamdichvu_thedv']", "0");
  await fillInput(page, "input[id$='txt_danhgia_dichvu_lamdv']", "Runtime service-card use");

  const ratingInput = page.locator("input[name='danhgia_5sao_nhanvien_dv_lamdv']").first();
  if (await ratingInput.count()) {
    await ratingInput.evaluate((element) => {
      element.value = "5";
      element.dispatchEvent(new Event("change", { bubbles: true }));
    });
  }

  await clickAndWait(page, page.locator("#form_themdvsp input[id$='Button1']").first());
  await waitForInvoiceRow(page, `Thẻ DV số ${card.cardId}`, beforeCount);

  await gotoWithRetry(page, `${baseUrl}/gianhang/admin/quan-ly-the-dich-vu/chi-tiet.aspx?id=${card.cardId}`, {
    waitUntil: "domcontentloaded",
  });
  await page.waitForLoadState("networkidle");

  const usageRows = page.locator("#nhatky tbody tr");
  const usageCount = await usageRows.count();
  if (usageCount === 0) {
    const stamp = Date.now();
    fs.writeFileSync(`/tmp/gianhang-admin-service-card-usage-${stamp}.html`, await page.content(), "utf8");
    throw new Error(
      `Service-card usage log is empty after using ${card.cardName}. dump=/tmp/gianhang-admin-service-card-usage-${stamp}.html`,
    );
  }
  const usageText = await usageRows.first().innerText().catch(() => "");
  if (!usageText.includes(serviceName)) {
    const stamp = Date.now();
    fs.writeFileSync(`/tmp/gianhang-admin-service-card-usage-${stamp}.html`, await page.content(), "utf8");
    throw new Error(
      `Service-card usage log did not mention ${serviceName} after using ${card.cardName}. row="${usageText}" dump=/tmp/gianhang-admin-service-card-usage-${stamp}.html`,
    );
  }

  await gotoWithRetry(page, `${baseUrl}/gianhang/admin/quan-ly-hoa-don/chi-tiet.aspx?id=${invoiceId}&q=add`, {
    waitUntil: "domcontentloaded",
  });
  await page.waitForLoadState("networkidle");
}

async function exerciseInvoiceDetail(page, card) {
  await gotoWithRetry(page, `${baseUrl}/gianhang/admin/quan-ly-hoa-don/chi-tiet.aspx?id=${invoiceId}&q=add`, {
    waitUntil: "domcontentloaded",
  });
  await page.waitForLoadState("networkidle");

  if (includeInvoiceDetail) {
    await addInvoiceService(page);
  }
  if (includeWarehouse) {
    await addInvoiceProduct(page);
  }
  if (includeServiceCard && card) {
    await useServiceCardOnInvoice(page, card);
  }
  if (includeInvoiceDetail || includeWarehouse) {
    await gotoWithRetry(page, `${baseUrl}/gianhang/admin/quan-ly-hoa-don/chi-tiet.aspx?id=${invoiceId}`, {
      waitUntil: "domcontentloaded",
    });
    await page.waitForLoadState("networkidle");
    console.log("Playwright: invoice payment flow");
    await payInvoice(page);
  }
}

async function main() {
  requireEnv(productName, "PRODUCT_NAME");
  requireEnv(serviceName, "SERVICE_NAME");
  requireEnv(invoiceId, "RUNTIME_INVOICE_ID");
  if (includeSupplyImport) {
    requireEnv(supplyName, "RUNTIME_VATTU_NAME");
  }
  if (includeServiceCard) {
    requireEnv(invoiceCustomerName, "RUNTIME_INVOICE_CUSTOMER_NAME");
    requireEnv(invoiceCustomerPhone, "RUNTIME_INVOICE_PHONE");
  }

  const executablePath = chromeCandidates.find((candidate) => candidate && fs.existsSync(candidate));
  const browser = await chromium.launch({
    headless: true,
    executablePath: executablePath || undefined,
  });

  try {
    const context = await browser.newContext({ ignoreHTTPSErrors: true });
    const page = await context.newPage();
    activePage = page;
    page.setDefaultTimeout(60000);
    page.setDefaultNavigationTimeout(60000);

    await login(page);

    let warehouseOrder = null;
    if (includeWarehouse) {
      console.log("Playwright: import warehouse flow");
      warehouseOrder = await importWarehouse(page);
      await payWarehouseOrder(page, warehouseOrder.orderId);
    }

    if (includeSupplyImport) {
      console.log("Playwright: import supply flow");
      const supplyOrder = await importSupplyOrder(page);
      await paySupplyOrder(page, supplyOrder.orderId);
    }

    if (includeStudent) {
      console.log("Playwright: student payment flow");
      const student = await createStudent(page);
      await payStudent(page, student);
    }

    let card = null;
    if (includeServiceCard) {
      console.log("Playwright: service-card sale flow");
      card = await sellServiceCard(page);
      await payServiceCard(page, card);
    }

    if (includeInvoiceDetail || includeWarehouse || includeServiceCard) {
      console.log("Playwright: invoice detail flow");
      await exerciseInvoiceDetail(page, card);
    }
  } finally {
    await browser.close();
  }
}

main().catch(async (error) => {
  if (activePage && !activePage.isClosed()) {
    const stamp = Date.now();
    try {
      await activePage.screenshot({ path: `/tmp/gianhang-admin-deep-${stamp}.png`, fullPage: true });
      const html = await activePage.content();
      fs.writeFileSync(`/tmp/gianhang-admin-deep-${stamp}.html`, html, "utf8");
    } catch (captureError) {
      console.error("Playwright debug capture failed:", captureError.message);
    }
  }
  console.error("Playwright deep flow failed:", error.message);
  process.exit(1);
});
