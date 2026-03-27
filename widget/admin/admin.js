(function () {
  "use strict";

  const els = {
    authGate: document.getElementById("auth-gate"),
    adminApp: document.getElementById("admin-app"),
    workspaceDependent: document.getElementById("workspace-dependent"),
    authHeader: document.getElementById("auth-header"),
    authWorkspaceBadge: document.getElementById("auth-workspace-badge"),
    authLogout: document.getElementById("auth-logout"),
    authRegisterUser: document.getElementById("auth-register-user"),
    authRegisterPassword: document.getElementById("auth-register-password"),
    authRegisterSubmit: document.getElementById("auth-register-submit"),
    authTabs: document.getElementById("auth-tabs"),
    authTabLogin: document.getElementById("auth-tab-login"),
    authTabRegister: document.getElementById("auth-tab-register"),
    authPanelLogin: document.getElementById("auth-panel-login"),
    authPanelRegister: document.getElementById("auth-panel-register"),
    authLoginId: document.getElementById("auth-login-id"),
    authLoginUser: document.getElementById("auth-login-user"),
    authLoginPassword: document.getElementById("auth-login-password"),
    authLoginSubmit: document.getElementById("auth-login-submit"),
    authStatus: document.getElementById("auth-status"),
    adminTabs: document.getElementById("admin-tabs"),
    workspaceTools: document.getElementById("workspace-tools"),
    workspaceToolsSummary: document.getElementById("workspace-tools-summary"),
    workspaceCreateName: document.getElementById("workspace-create-name"),
    workspaceCreateId: document.getElementById("workspace-create-id"),
    workspaceCreateSubmit: document.getElementById("workspace-create-submit"),
    workspaceSwitchSelect: document.getElementById("workspace-switch-select"),
    workspaceSwitchSubmit: document.getElementById("workspace-switch-submit"),
    quickBrandName: document.getElementById("quick-brand-name"),
    quickSiteUrl: document.getElementById("quick-site-url"),
    quickAgentName: document.getElementById("quick-agent-name"),
    quickWelcomeMessage: document.getElementById("quick-welcome-message"),
    quickFallbackAnswer: document.getElementById("quick-fallback-answer"),
    quickLoginUrl: document.getElementById("quick-login-url"),
    quickLoginUsername: document.getElementById("quick-login-username"),
    quickLoginPassword: document.getElementById("quick-login-password"),
    quickLoginOtp: document.getElementById("quick-login-otp"),
    quickTogglePassword: document.getElementById("quick-toggle-password"),
    quickAutoDeploy: document.getElementById("quick-auto-deploy"),
    quickCopyEmbed: document.getElementById("quick-copy-embed"),
    quickOpenDemo: document.getElementById("quick-open-demo"),
    quickEmbedCode: document.getElementById("quick-embed-code"),
    quickStatus: document.getElementById("quick-status"),
    brandName: document.getElementById("brand-name"),
    agentName: document.getElementById("agent-name"),
    launcherLabel: document.getElementById("launcher-label"),
    welcomeTitle: document.getElementById("welcome-title"),
    welcomeMessage: document.getElementById("welcome-message"),
    inputPlaceholder: document.getElementById("input-placeholder"),
    logoPreview: document.getElementById("logo-preview"),
    logoUpload: document.getElementById("logo-upload"),
    logoClear: document.getElementById("logo-clear"),
    themePrimary: document.getElementById("theme-primary"),
    themeLauncherBg: document.getElementById("theme-launcher-bg"),
    themeTextOnPrimary: document.getElementById("theme-text-on-primary"),
    themePanelBg: document.getElementById("theme-panel-bg"),
    themeBotBubble: document.getElementById("theme-bot-bubble"),
    themeUserBubble: document.getElementById("theme-user-bubble"),
    layoutPosition: document.getElementById("layout-position"),
    layoutBottom: document.getElementById("layout-bottom"),
    layoutSide: document.getElementById("layout-side"),
    layoutZIndex: document.getElementById("layout-z-index"),
    behaviorShowWelcome: document.getElementById("behavior-show-welcome"),
    behaviorStartOpen: document.getElementById("behavior-start-open"),
    behaviorTypingDelay: document.getElementById("behavior-typing-delay"),
    behaviorMaxMessages: document.getElementById("behavior-max-messages"),
    quickStartButtons: document.getElementById("quick-start-buttons"),
    fallbackAnswer: document.getElementById("fallback-answer"),
    kbList: document.getElementById("kb-list"),
    addKbRow: document.getElementById("add-kb-row"),
    kbUpload: document.getElementById("kb-upload"),
    kbImport: document.getElementById("kb-import"),
    kbExportCsv: document.getElementById("kb-export-csv"),
    kbTemplateXlsx: document.getElementById("kb-template-xlsx"),
    kbExportXlsx: document.getElementById("kb-export-xlsx"),
    saveConfig: document.getElementById("save-config"),
    advancedSaveConfig: document.getElementById("advanced-save-config"),
    reloadConfig: document.getElementById("reload-config"),
    bundleDownload: document.getElementById("bundle-download"),
    bundleUpload: document.getElementById("bundle-upload"),
    bundleImport: document.getElementById("bundle-import"),
    directDeployKey: document.getElementById("direct-deploy-key"),
    directToggleDeployKey: document.getElementById("direct-toggle-deploy-key"),
    directDeployTest: document.getElementById("direct-deploy-test"),
    directDeploySubmit: document.getElementById("direct-deploy-submit"),
    directDeployStatus: document.getElementById("direct-deploy-status"),
    status: document.getElementById("status"),
    embedCode: document.getElementById("embed-code"),
    copyEmbed: document.getElementById("copy-embed"),
    docTitle: document.getElementById("doc-title"),
    docTags: document.getElementById("doc-tags"),
    docText: document.getElementById("doc-text"),
    addDocument: document.getElementById("add-document"),
    docUpload: document.getElementById("doc-upload"),
    uploadDocuments: document.getElementById("upload-documents"),
    reloadDocuments: document.getElementById("reload-documents"),
    documentsList: document.getElementById("documents-list"),
    documentsTotal: document.getElementById("documents-total"),
    documentsSearch: document.getElementById("documents-search"),
    documentsClearSearch: document.getElementById("documents-clear-search"),
    documentsPageSize: document.getElementById("documents-page-size"),
    documentsVisible: document.getElementById("documents-visible"),
    documentsPageCurrent: document.getElementById("documents-page-current"),
    documentsPageTotal: document.getElementById("documents-page-total"),
    documentsPagePrev: document.getElementById("documents-page-prev"),
    documentsPageNext: document.getElementById("documents-page-next"),
    documentsExportCsv: document.getElementById("documents-export-csv"),
    documentsTemplateXlsx: document.getElementById("documents-template-xlsx"),
    documentsExportXlsx: document.getElementById("documents-export-xlsx"),
    platformProjectId: document.getElementById("platform-project-id"),
    platformProjectName: document.getElementById("platform-project-name"),
    platformBaseUrl: document.getElementById("platform-base-url"),
    platformMaxPages: document.getElementById("platform-max-pages"),
    platformMaxDepth: document.getElementById("platform-max-depth"),
    platformTimeoutMs: document.getElementById("platform-timeout-ms"),
    platformIncludePaths: document.getElementById("platform-include-paths"),
    platformExcludePaths: document.getElementById("platform-exclude-paths"),
    platformAuthHeaders: document.getElementById("platform-auth-headers"),
    platformSaveProject: document.getElementById("platform-save-project"),
    platformRunCrawl: document.getElementById("platform-run-crawl"),
    platformPublishGuides: document.getElementById("platform-publish-guides"),
    platformLoadProject: document.getElementById("platform-load-project"),
    platformSummary: document.getElementById("platform-summary"),
    superAdminTools: document.getElementById("super-admin-tools"),
    superRefreshAccounts: document.getElementById("super-refresh-accounts"),
    superAccountsList: document.getElementById("super-accounts-list"),
    reviewLoadDraft: document.getElementById("review-load-draft"),
    reviewCheckAll: document.getElementById("review-check-all"),
    reviewSaveDraft: document.getElementById("review-save-draft"),
    reviewPublishGuides: document.getElementById("review-publish-guides"),
    reviewSummary: document.getElementById("review-summary"),
    reviewGuidesList: document.getElementById("review-guides-list")
  };

  const DEFAULT_CONFIG = {
    brand: {
      name: "Aha Shine",
      agentName: "Trợ lý Aha",
      launcherLabel: "Aha Shine",
      welcomeTitle: "Xin chào, mình có thể hỗ trợ gì?",
      welcomeMessage: "Mời bạn chọn nhanh nội dung bên dưới hoặc nhập câu hỏi.",
      inputPlaceholder: "Nhập tin nhắn...",
      logoDataUrl: ""
    },
    theme: {
      primaryColor: "#39b54a",
      launcherBg: "#ffd33d",
      textOnPrimary: "#ffffff",
      panelBg: "#ffffff",
      botBubbleBg: "#f2f4f7",
      userBubbleBg: "#fff6d8"
    },
    layout: {
      position: "right",
      bottom: 20,
      side: 20,
      zIndex: 2147482000
    },
    behavior: {
      showWelcomeOnOpen: true,
      startOpen: false,
      maxMessages: 120,
      typingDelayMs: 300
    },
    quickStartButtons: ["Bảng giá", "Khuyến mãi", "Đặt lịch", "Liên hệ"],
    knowledgeBase: [],
    fallbackAnswer:
      "Mình đã ghi nhận câu hỏi của bạn. Bạn có thể để lại số điện thoại để nhân viên hỗ trợ chi tiết hơn nhé."
  };

  const STATIC_BUNDLE_URL = "../widget-data/widget-data.json";
  const STATIC_BUNDLE_CACHE_KEY = "__AHA_WIDGET_STATIC_BUNDLE_V1__";
  const STATIC_SESSION_KEY = "__AHA_WIDGET_STATIC_SESSION_V1__";
  const DIRECT_DEPLOY_KEY_STORAGE = "__AHA_WIDGET_DIRECT_DEPLOY_KEY_V1__";
  const DIRECT_DEPLOY_ENDPOINT = "./deploy.aspx";
  const DEFAULT_SUPER_ADMIN = {
    username: "root",
    password: "Aha@123456"
  };
  const ADMIN_TAB_STORAGE_KEY = "__AHA_WIDGET_ADMIN_TAB_V1__";
  const ADMIN_TAB_IDS = ["setup", "appearance", "content"];
  const AUTH_TAB_IDS = ["login", "register"];

  let currentLogoDataUrl = "";
  let documentItems = [];
  let documentsSearchQuery = "";
  let documentsPageSize = 20;
  let documentsCurrentPage = 1;
  let reviewDraftItems = [];
  let currentCrawlStatus = "idle";
  let staticBundle = null;
  let currentUserRole = "";
  let currentAccountId = "";
  let currentAccountUsername = "";
  const AUTH_TOKEN_STORAGE_KEY = "__AHA_WIDGET_ADMIN_TOKEN__";
  let authToken = "";
  let currentWorkspace = null;
  let activeAdminTab = ADMIN_TAB_IDS[0];
  let activeAuthTab = AUTH_TAB_IDS[0];

  const nativeFetch = window.fetch.bind(window);
  window.fetch = function fetchWithAuth(input, init) {
    const nextInit = init && typeof init === "object" ? { ...init } : {};
    if (!authToken) {
      return nativeFetch(input, nextInit);
    }
    let requestUrl = "";
    if (typeof input === "string") {
      requestUrl = input;
    } else if (input && typeof input === "object" && typeof input.url === "string") {
      requestUrl = input.url;
    }
    try {
      const parsed = new URL(requestUrl || "/", window.location.origin);
      const isSameOrigin = parsed.origin === window.location.origin;
      const isApiRoute = parsed.pathname.startsWith("/api/");
      const isPublicApi = parsed.pathname.startsWith("/api/public/");
      const isOpenAuthRoute =
        parsed.pathname === "/api/auth/login" ||
        parsed.pathname === "/api/auth/register-workspace" ||
        parsed.pathname === "/api/auth/workspaces";
      if (!isSameOrigin || !isApiRoute || isPublicApi || isOpenAuthRoute) {
        return nativeFetch(input, nextInit);
      }
      const headers = new Headers(nextInit.headers || {});
      headers.set("Authorization", `Bearer ${authToken}`);
      nextInit.headers = headers;
      return nativeFetch(input, nextInit);
    } catch (_) {
      return nativeFetch(input, nextInit);
    }
  };

  function readStoredAuthToken() {
    try {
      return localStorage.getItem(AUTH_TOKEN_STORAGE_KEY) || "";
    } catch (_) {
      return "";
    }
  }

  function writeStoredAuthToken(token) {
    try {
      if (token) {
        localStorage.setItem(AUTH_TOKEN_STORAGE_KEY, token);
      } else {
        localStorage.removeItem(AUTH_TOKEN_STORAGE_KEY);
      }
    } catch (_) {}
  }

  function parseJsonSafely(text) {
    try {
      return JSON.parse(text);
    } catch (_) {
      return null;
    }
  }

  async function buildHttpError(response, fallbackMessage) {
    const bodyText = await response.text();
    const parsed = parseJsonSafely(bodyText);
    const serverMessage =
      parsed && typeof parsed.message === "string" ? parsed.message.trim() : "";
    const message = serverMessage || `${fallbackMessage} (HTTP ${response.status}).`;
    return new Error(message);
  }

  function setStatus(message, type) {
    els.status.textContent = message;
    if (type === "error") {
      els.status.style.color = "#c62828";
      return;
    }
    if (type === "success") {
      els.status.style.color = "#0f8a33";
      return;
    }
    els.status.style.color = "#5b6578";
  }

  function setQuickStatus(message, type) {
    if (!els.quickStatus) {
      return;
    }
    els.quickStatus.textContent = message;
    els.quickStatus.classList.remove("success", "error");
    if (type === "success") {
      els.quickStatus.classList.add("success");
      return;
    }
    if (type === "error") {
      els.quickStatus.classList.add("error");
    }
  }

  function setDirectDeployStatus(message, type) {
    if (!els.directDeployStatus) {
      return;
    }
    els.directDeployStatus.textContent = message;
    els.directDeployStatus.classList.remove("success", "error");
    if (type === "success") {
      els.directDeployStatus.classList.add("success");
      return;
    }
    if (type === "error") {
      els.directDeployStatus.classList.add("error");
    }
  }

  function setAuthStatus(message, type) {
    if (!els.authStatus) {
      return;
    }
    els.authStatus.textContent = message;
    els.authStatus.classList.remove("success", "error");
    if (type === "success") {
      els.authStatus.classList.add("success");
      return;
    }
    if (type === "error") {
      els.authStatus.classList.add("error");
    }
  }

  function readStoredDeployKey() {
    try {
      return localStorage.getItem(DIRECT_DEPLOY_KEY_STORAGE) || "";
    } catch (_) {
      return "";
    }
  }

  function writeStoredDeployKey(value) {
    try {
      const text = normalizeText(value, "", 240);
      if (!text) {
        localStorage.removeItem(DIRECT_DEPLOY_KEY_STORAGE);
        return;
      }
      localStorage.setItem(DIRECT_DEPLOY_KEY_STORAGE, text);
    } catch (_) {}
  }

  function updateWorkspaceBadge() {
    if (!els.authWorkspaceBadge) {
      return;
    }
    if (currentUserRole === "superadmin") {
      els.authWorkspaceBadge.textContent = "Super Admin | Quản trị tài khoản";
      return;
    }
    if (currentWorkspace && currentWorkspace.id) {
      const name = normalizeText(currentWorkspace.name, currentWorkspace.id, 120);
      const roleText = currentUserRole === "superadmin" ? "Super Admin" : "User";
      els.authWorkspaceBadge.textContent = `${roleText} | Workspace: ${name} (${currentWorkspace.id})`;
      return;
    }
    if (currentUserRole) {
      const roleText = currentUserRole === "superadmin" ? "Super Admin" : "User";
      els.authWorkspaceBadge.textContent = `${roleText} | Workspace: chưa chọn`;
      return;
    }
    els.authWorkspaceBadge.textContent = "Workspace: -";
  }

  function setAuthenticatedView(isLoggedIn) {
    const loggedIn = Boolean(isLoggedIn);
    if (els.authGate) {
      els.authGate.hidden = loggedIn;
    }
    if (els.adminApp) {
      els.adminApp.hidden = !loggedIn;
    }
    if (els.authHeader) {
      els.authHeader.hidden = !loggedIn;
    }
    if (els.superAdminTools) {
      els.superAdminTools.hidden = !(loggedIn && currentUserRole === "superadmin");
    }
    if (els.workspaceTools) {
      els.workspaceTools.hidden = !loggedIn || currentUserRole !== "user";
    }
    updateRoleBasedAdminLayout();
    updateWorkspaceBadge();
  }

  function setWorkspaceDependentVisible(visible) {
    if (els.workspaceDependent) {
      els.workspaceDependent.hidden = !visible;
    }
  }

  function isValidAuthTab(tabId) {
    return AUTH_TAB_IDS.includes(normalizeText(tabId, "", 40));
  }

  function setActiveAuthTab(tabId) {
    const nextTab = isValidAuthTab(tabId) ? tabId : AUTH_TAB_IDS[0];
    activeAuthTab = nextTab;
    const buttons = [els.authTabLogin, els.authTabRegister].filter(Boolean);
    buttons.forEach((button) => {
      if (!(button instanceof HTMLButtonElement)) {
        return;
      }
      const ownsTab = normalizeText(button.getAttribute("data-auth-tab"), "", 40) === nextTab;
      button.classList.toggle("is-active", ownsTab);
      button.setAttribute("aria-selected", ownsTab ? "true" : "false");
    });
    if (els.authPanelLogin) {
      els.authPanelLogin.hidden = nextTab !== "login";
    }
    if (els.authPanelRegister) {
      els.authPanelRegister.hidden = nextTab !== "register";
    }
  }

  function updateRoleBasedAdminLayout() {
    const isSuperAdmin = currentUserRole === "superadmin";
    if (els.adminTabs) {
      els.adminTabs.hidden = isSuperAdmin;
    }
    const panels = getAdminTabPanels();
    panels.forEach((panel) => {
      if (!(panel instanceof HTMLElement)) {
        return;
      }
      const isSuperAdminPanel = panel.id === "super-admin-tools";
      if (isSuperAdmin) {
        if (isSuperAdminPanel) {
          panel.hidden = false;
          panel.classList.remove("tab-hidden");
        } else {
          panel.hidden = true;
          panel.classList.add("tab-hidden");
        }
        return;
      }
      if (isSuperAdminPanel) {
        panel.hidden = true;
        panel.classList.add("tab-hidden");
        return;
      }
      panel.hidden = false;
    });
  }

  function isValidAdminTab(tabId) {
    return ADMIN_TAB_IDS.includes(normalizeText(tabId, "", 40));
  }

  function readStoredAdminTab() {
    try {
      const value = localStorage.getItem(ADMIN_TAB_STORAGE_KEY) || "";
      return isValidAdminTab(value) ? value : ADMIN_TAB_IDS[0];
    } catch (_) {
      return ADMIN_TAB_IDS[0];
    }
  }

  function writeStoredAdminTab(tabId) {
    try {
      const value = isValidAdminTab(tabId) ? tabId : ADMIN_TAB_IDS[0];
      localStorage.setItem(ADMIN_TAB_STORAGE_KEY, value);
    } catch (_) {}
  }

  function getAdminTabButtons() {
    return Array.from(document.querySelectorAll("[data-admin-tab]"));
  }

  function getAdminTabPanels() {
    return Array.from(document.querySelectorAll("#workspace-dependent [data-tab-panel]"));
  }

  function setActiveAdminTab(tabId, options) {
    const opts = options && typeof options === "object" ? options : {};
    const nextTab = isValidAdminTab(tabId) ? tabId : ADMIN_TAB_IDS[0];
    activeAdminTab = nextTab;
    const buttons = getAdminTabButtons();
    buttons.forEach((button) => {
      if (!(button instanceof HTMLButtonElement)) {
        return;
      }
      const ownsTab = normalizeText(button.getAttribute("data-admin-tab"), "", 40) === nextTab;
      button.classList.toggle("is-active", ownsTab);
      button.setAttribute("aria-selected", ownsTab ? "true" : "false");
    });
    const panels = getAdminTabPanels();
    panels.forEach((panel) => {
      if (!(panel instanceof HTMLElement)) {
        return;
      }
      const panelTab = normalizeText(panel.getAttribute("data-tab-panel"), "", 40);
      panel.classList.toggle("tab-hidden", panelTab !== nextTab);
    });
    if (opts.persist !== false) {
      writeStoredAdminTab(nextTab);
    }
    updateRoleBasedAdminLayout();
  }

  function initializeCardCollapsers() {
    const cards = Array.from(document.querySelectorAll("#admin-app section.card"));
    cards.forEach((card, index) => {
      if (!(card instanceof HTMLElement)) {
        return;
      }
      if (card.dataset.collapseReady === "1") {
        return;
      }
      let head = card.querySelector(":scope > .card-head");
      let title = "";
      if (head) {
        const heading = head.querySelector("h2");
        title = normalizeText(heading && heading.textContent ? heading.textContent : "", `Khối ${index + 1}`, 120);
      } else {
        const heading = card.querySelector(":scope > h2");
        title = normalizeText(heading && heading.textContent ? heading.textContent : "", `Khối ${index + 1}`, 120);
        const generatedHead = document.createElement("div");
        generatedHead.className = "card-head";
        if (heading && heading.parentElement === card) {
          generatedHead.appendChild(heading);
        } else {
          const generatedTitle = document.createElement("h2");
          generatedTitle.textContent = title || `Khối ${index + 1}`;
          generatedHead.appendChild(generatedTitle);
        }
        card.insertBefore(generatedHead, card.firstChild);
        head = generatedHead;
      }
      if (!(head instanceof HTMLElement)) {
        return;
      }
      head.classList.add("card-head-collapsible");
      const toggle = document.createElement("button");
      toggle.type = "button";
      toggle.className = "card-collapse-btn";
      toggle.textContent = "－";
      toggle.setAttribute("aria-expanded", "true");
      toggle.setAttribute("aria-label", `Thu gọn hoặc mở rộng: ${title || `Khối ${index + 1}`}`);
      head.appendChild(toggle);
      card.classList.add("card-collapsible");
      card.dataset.collapseReady = "1";

      toggle.addEventListener("click", () => {
        const collapsed = card.classList.toggle("card-collapsed");
        toggle.textContent = collapsed ? "＋" : "－";
        toggle.setAttribute("aria-expanded", collapsed ? "false" : "true");
      });
    });
  }

  function clamp(number, min, max, fallback) {
    const value = Number(number);
    if (!Number.isFinite(value)) {
      return fallback;
    }
    return Math.min(max, Math.max(min, Math.round(value)));
  }

  function normalizeText(value, fallback = "", max = 400) {
    if (typeof value !== "string") {
      return fallback;
    }
    const text = value.trim();
    if (!text) {
      return fallback;
    }
    return text.slice(0, max);
  }

  function foldSearchText(value) {
    return (value || "")
      .toString()
      .normalize("NFD")
      .replace(/[\u0300-\u036f]/g, "")
      .replace(/[đĐ]/g, "d")
      .toLowerCase()
      .replace(/\s+/g, " ")
      .trim();
  }

  function escapeCsvCell(value) {
    const text = (value || "").toString();
    if (text.includes("\"")) {
      return `"${text.replace(/"/g, "\"\"")}"`;
    }
    if (text.includes(",") || text.includes("\n") || text.includes("\r")) {
      return `"${text}"`;
    }
    return text;
  }

  function buildCsvLine(columns) {
    return columns.map((value) => escapeCsvCell(value)).join(",");
  }

  function downloadTextFile(fileName, content, mimeType) {
    const blob = new Blob([content], { type: mimeType || "text/plain;charset=utf-8" });
    const url = URL.createObjectURL(blob);
    const anchor = document.createElement("a");
    anchor.href = url;
    anchor.download = fileName;
    document.body.appendChild(anchor);
    anchor.click();
    anchor.remove();
    URL.revokeObjectURL(url);
  }

  function getXlsxLib() {
    const lib = window.XLSX;
    if (!lib || !lib.utils) {
      throw new Error("Thiếu thư viện XLSX. Hãy tải lại trang và kiểm tra kết nối Internet.");
    }
    return lib;
  }

  async function readFileAsArrayBuffer(file) {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.onload = () => resolve(reader.result);
      reader.onerror = () => reject(new Error(`Không thể đọc file: ${file.name}`));
      reader.readAsArrayBuffer(file);
    });
  }

  function normalizeSheetRows(rows) {
    return (Array.isArray(rows) ? rows : []).map((row) => {
      const source = row && typeof row === "object" ? row : {};
      const normalized = {};
      Object.keys(source).forEach((key) => {
        const normalizedKey = normalizeText(key, "", 80).toLowerCase().replace(/\s+/g, "");
        if (!normalizedKey) {
          return;
        }
        normalized[normalizedKey] = source[key];
      });
      return normalized;
    });
  }

  function parseKbFromXlsxRows(rows) {
    const normalizedRows = normalizeSheetRows(rows);
    const output = [];
    normalizedRows.forEach((row, index) => {
      const answer = normalizeText((row.answer || "").toString(), "", 2000);
      if (!answer) {
        return;
      }
      const id = normalizeText((row.id || "").toString(), `kb_${index + 1}`, 80);
      const keywords = parseFlexibleList((row.keywords || "").toString()).slice(0, 30);
      const quickReplies = parseFlexibleList((row.quickreplies || "").toString()).slice(0, 8);
      output.push({
        id,
        keywords,
        answer,
        quickReplies
      });
    });
    return output;
  }

  function parseDocumentsFromXlsxRows(rows) {
    const normalizedRows = normalizeSheetRows(rows);
    const output = [];
    normalizedRows.forEach((row, index) => {
      const textValue = normalizeText((row.text || "").toString(), "", 120000);
      if (!textValue) {
        return;
      }
      const title = normalizeText((row.title || "").toString(), `Tài liệu ${index + 1}`, 180);
      const id = normalizeText((row.id || "").toString(), "", 160);
      const tags = parseFlexibleList((row.tags || "").toString()).slice(0, 20);
      const sourceUrls = parseFlexibleList((row.sourceurls || "").toString())
        .map((url) => normalizeUrlInput(url))
        .filter(Boolean)
        .slice(0, 10);
      output.push({
        id,
        title,
        text: textValue,
        tags,
        sourceUrls
      });
    });
    return output;
  }

  async function parseXlsxFile(file) {
    const XLSX = getXlsxLib();
    const buffer = await readFileAsArrayBuffer(file);
    const workbook = XLSX.read(buffer, { type: "array" });
    const firstSheetName = Array.isArray(workbook.SheetNames) ? workbook.SheetNames[0] : "";
    if (!firstSheetName || !workbook.Sheets[firstSheetName]) {
      throw new Error("File XLSX không có sheet dữ liệu hợp lệ.");
    }
    const sheet = workbook.Sheets[firstSheetName];
    return XLSX.utils.sheet_to_json(sheet, { defval: "", raw: false });
  }

  function downloadXlsxFile(fileName, rows, sheetName) {
    const XLSX = getXlsxLib();
    const workbook = XLSX.utils.book_new();
    const safeRows = Array.isArray(rows) ? rows : [];
    const worksheet = XLSX.utils.json_to_sheet(safeRows);
    XLSX.utils.book_append_sheet(workbook, worksheet, normalizeText(sheetName, "Sheet1", 28));
    XLSX.writeFile(workbook, fileName);
  }

  function normalizeDocumentsPageSize(value) {
    const parsed = clamp(value, 10, 20, 20);
    return parsed <= 10 ? 10 : 20;
  }

  function slugify(value, fallback = "default-site") {
    const text = normalizeText(value, fallback, 120)
      .toLowerCase()
      .replace(/[^\w-]+/g, "-")
      .replace(/-+/g, "-")
      .replace(/^-+|-+$/g, "");
    return text || fallback;
  }

  function normalizeUrlInput(value) {
    const raw = normalizeText(value, "", 400);
    if (!raw) {
      return "";
    }
    const source = /^[a-z]+:\/\//i.test(raw) ? raw : `https://${raw}`;
    try {
      const parsed = new URL(source);
      if (!/^https?:$/i.test(parsed.protocol)) {
        return "";
      }
      parsed.hash = "";
      return parsed.href;
    } catch (_) {
      return "";
    }
  }

  function deriveProjectIdFromUrl(urlString) {
    const normalizedUrl = normalizeUrlInput(urlString);
    if (!normalizedUrl) {
      return "default-site";
    }
    try {
      const parsed = new URL(normalizedUrl);
      const hostPart = slugify(parsed.hostname.replace(/^www\./i, "").replace(/\./g, "-"), "site");
      const firstPath = parsed.pathname
        .split("/")
        .map((item) => item.trim())
        .filter(Boolean)[0];
      if (!firstPath) {
        return hostPart;
      }
      const pathPart = slugify(firstPath, "");
      return slugify(pathPart ? `${hostPart}-${pathPart}` : hostPart, hostPart);
    } catch (_) {
      return "default-site";
    }
  }

  function buildDefaultLoginUrl(baseUrl) {
    const normalized = normalizeUrlInput(baseUrl);
    if (!normalized) {
      return "";
    }
    try {
      return new URL("/dang-nhap", normalized).href;
    } catch (_) {
      return "";
    }
  }

  function getAllAccounts() {
    return staticBundle && staticBundle.auth && Array.isArray(staticBundle.auth.accounts)
      ? staticBundle.auth.accounts
      : [];
  }

  function getAllWorkspaces() {
    return Array.isArray(staticBundle && staticBundle.workspaces) ? staticBundle.workspaces : [];
  }

  function workspaceExists(workspaceId) {
    const normalized = slugify(normalizeText(workspaceId, "", 120), "");
    if (!normalized) {
      return false;
    }
    return getAllWorkspaces().some((item) => item && item.id === normalized);
  }

  function normalizeWorkspaceIdList(values) {
    const source = Array.isArray(values) ? values : [];
    const unique = [];
    const seen = new Set();
    source.forEach((item) => {
      const id = slugify(normalizeText(item, "", 120), "");
      if (!id || seen.has(id)) {
        return;
      }
      seen.add(id);
      unique.push(id);
    });
    return unique;
  }

  function normalizeAccountWorkspaceIds(values) {
    return normalizeWorkspaceIdList(values).filter((workspaceId) => workspaceExists(workspaceId));
  }

  function buildAccountIdFromUsername(username) {
    const base = slugify(normalizeText(username, "user", 120), "user");
    const existingIds = new Set(getAllAccounts().map((item) => normalizeText(item && item.id, "", 120)));
    let candidate = `acc_${base}`;
    let suffix = 2;
    while (existingIds.has(candidate)) {
      candidate = `acc_${base}_${suffix}`;
      suffix += 1;
    }
    return candidate;
  }

  function findAccountById(accountId) {
    const normalized = normalizeText(accountId, "", 120);
    if (!normalized) {
      return null;
    }
    return getAllAccounts().find((item) => normalizeText(item && item.id, "", 120) === normalized) || null;
  }

  function findAccountByUsername(username) {
    const normalized = normalizeText(username, "", 120);
    if (!normalized) {
      return null;
    }
    const normalizedKey = normalized.toLowerCase();
    return getAllAccounts()
      .find((item) => normalizeText(item && item.username, "", 120).toLowerCase() === normalizedKey) || null;
  }

  function isAccountLocked(account) {
    return Boolean(account && typeof account === "object" && account.locked === true);
  }

  function updateAccountById(accountId, updater) {
    const normalizedAccountId = normalizeText(accountId, "", 120);
    if (!normalizedAccountId || typeof updater !== "function") {
      return null;
    }
    const accounts = getAllAccounts();
    let updatedAccount = null;
    staticBundle.auth.accounts = accounts.map((account) => {
      const id = normalizeText(account && account.id, "", 120);
      if (id !== normalizedAccountId) {
        return account;
      }
      const next = updater(account && typeof account === "object" ? { ...account } : null);
      updatedAccount = next && typeof next === "object" ? next : account;
      return updatedAccount;
    });
    return updatedAccount;
  }

  function setAccountLockedState(accountId, locked) {
    const nextLocked = Boolean(locked);
    const updated = updateAccountById(accountId, (account) => {
      if (!account) {
        return null;
      }
      return {
        ...account,
        locked: nextLocked,
        lockedAt: nextLocked ? new Date().toISOString() : ""
      };
    });
    if (!updated) {
      throw new Error("Không tìm thấy tài khoản để cập nhật trạng thái khóa.");
    }
    persistBundleState();
    return updated;
  }

  function getCurrentAccount() {
    if (!currentAccountId) {
      return findAccountByUsername(currentAccountUsername);
    }
    return findAccountById(currentAccountId) || findAccountByUsername(currentAccountUsername);
  }

  function accountHasWorkspace(account, workspaceId) {
    const normalizedWorkspaceId = slugify(normalizeText(workspaceId, "", 120), "");
    if (!account || !normalizedWorkspaceId) {
      return false;
    }
    const workspaceIds = normalizeWorkspaceIdList(account.workspaceIds);
    return workspaceIds.includes(normalizedWorkspaceId);
  }

  function getAccessibleWorkspacesForCurrentUser() {
    if (currentUserRole === "superadmin") {
      return [];
    }
    const allWorkspaces = getAllWorkspaces();
    const account = getCurrentAccount();
    if (!account) {
      return [];
    }
    const idSet = new Set(normalizeAccountWorkspaceIds(account.workspaceIds));
    return allWorkspaces.filter((item) => item && idSet.has(item.id));
  }

  function applyAuthSessionPayload(payload) {
    const source = payload && typeof payload === "object" ? payload : {};
    const role = normalizeText(source.role, "user", 30).toLowerCase() === "superadmin" ? "superadmin" : "user";
    currentUserRole = role;
    const account = source.account && typeof source.account === "object" ? source.account : {};
    if (role === "user") {
      currentAccountId = normalizeText(account.id, "", 120);
      currentAccountUsername = normalizeText(account.username, "", 120);
    } else {
      currentAccountId = "";
      currentAccountUsername = "";
    }
    const workspace = source.workspace && typeof source.workspace === "object" ? source.workspace : null;
    let resolvedWorkspace = null;
    if (workspace && workspace.id) {
      const workspaceId = slugify(normalizeText(workspace.id, "", 120), "");
      if (workspaceId) {
        try {
          setActiveWorkspace(workspaceId);
          resolvedWorkspace = {
            id: workspaceId,
            name: normalizeText(workspace.name, workspaceId, 160),
            widgetKey: normalizeText(workspace.widgetKey, `wk_${workspaceId}`, 120),
            adminUser: normalizeText(
              workspace.adminUser,
              role === "superadmin"
                ? normalizeText(workspace.adminUser, DEFAULT_SUPER_ADMIN.username, 120)
                : normalizeText(currentAccountUsername, "admin", 120),
              120
            )
          };
        } catch (_) {
          resolvedWorkspace = null;
        }
      }
    }
    if (!resolvedWorkspace) {
      currentWorkspace = null;
      if (staticBundle && typeof staticBundle === "object" && role !== "superadmin") {
        staticBundle.activeWorkspaceId = "";
      }
    } else {
      currentWorkspace = resolvedWorkspace;
    }
    authToken = `local-static-token-${Date.now()}`;
    writeStoredAuthToken(authToken);
    saveSession({
      role: currentUserRole,
      workspaceId: normalizeText(currentWorkspace && currentWorkspace.id ? currentWorkspace.id : "", "", 120),
      adminUser: normalizeText(currentWorkspace && currentWorkspace.adminUser ? currentWorkspace.adminUser : "", "", 120),
      accountId: normalizeText(currentAccountId, "", 120),
      username: normalizeText(currentAccountUsername, "", 120)
    });
    updateWorkspaceBadge();
    renderEmbedCode();
    renderQuickEmbedPreview();
    renderWorkspaceToolsPanel();
    renderSuperAdminAccountsPanel();
    updateRoleBasedAdminLayout();
    setWorkspaceDependentVisible(Boolean(currentWorkspace && currentWorkspace.id));
    return currentWorkspace;
  }

  async function registerWorkspaceAccount() {
    ensureBundleState();
    const adminUser = normalizeText(els.authRegisterUser ? els.authRegisterUser.value : "", "", 120);
    const password = normalizeText(els.authRegisterPassword ? els.authRegisterPassword.value : "", "", 200);
    if (!adminUser || !password) {
      throw new Error("Bạn cần nhập đủ tài khoản và mật khẩu.");
    }
    const existingAccount = findAccountByUsername(adminUser);
    if (existingAccount) {
      throw new Error("Tài khoản đã tồn tại. Hãy dùng tên khác.");
    }
    const superAdmins = staticBundle.auth && Array.isArray(staticBundle.auth.superAdmins)
      ? staticBundle.auth.superAdmins
      : [];
    const adminKey = adminUser.toLowerCase();
    const hasReservedUsername = superAdmins
      .some((item) => normalizeText(item && item.username, "", 120).toLowerCase() === adminKey);
    if (hasReservedUsername) {
      throw new Error("Tên tài khoản này đang được dùng cho Super Admin. Hãy chọn tên khác.");
    }
    const accountId = buildAccountIdFromUsername(adminUser);
    staticBundle.auth.accounts = [
      ...getAllAccounts(),
      {
        id: accountId,
        username: adminUser,
        displayName: adminUser,
        password,
        workspaceIds: [],
        locked: false,
        lockedAt: ""
      }
    ];
    persistBundleState();
    return {
      role: "user",
      account: {
        id: accountId,
        username: adminUser
      }
    };
  }

  async function loginWorkspaceAccount() {
    ensureBundleState();
    const workspaceId = slugify(normalizeText(els.authLoginId ? els.authLoginId.value : "", "", 120), "");
    const adminUser = normalizeText(els.authLoginUser ? els.authLoginUser.value : "", "", 120);
    const password = normalizeText(els.authLoginPassword ? els.authLoginPassword.value : "", "", 200);
    if (!adminUser || !password) {
      throw new Error("Bạn cần nhập tài khoản và mật khẩu.");
    }

    const superAdminMatched = (staticBundle.auth && Array.isArray(staticBundle.auth.superAdmins)
      ? staticBundle.auth.superAdmins
      : [])
      .find((item) => normalizeText(item && item.username, "", 120).toLowerCase() === adminUser.toLowerCase()
        && normalizeText(item && item.password, "", 200) === password);
    if (superAdminMatched) {
      return {
        role: "superadmin",
        token: `local-static-token-${Date.now()}`,
        workspace: null
      };
    }

    const account = getAllAccounts()
      .find((item) => normalizeText(item && item.username, "", 120).toLowerCase() === adminUser.toLowerCase()
        && normalizeText(item && item.password, "", 200) === password) || null;
    if (!account) {
      throw new Error("Sai tài khoản hoặc mật khẩu.");
    }
    if (isAccountLocked(account)) {
      throw new Error("Tài khoản này đang bị khóa. Vui lòng liên hệ Super Admin.");
    }

    const accountWorkspaceIds = normalizeAccountWorkspaceIds(account.workspaceIds);
    let targetWorkspace = null;
    if (workspaceId) {
      if (!accountWorkspaceIds.includes(workspaceId)) {
        throw new Error("Bạn không có quyền vào workspace này.");
      }
      targetWorkspace = getAllWorkspaces().find((item) => item.id === workspaceId) || null;
      if (!targetWorkspace) {
        throw new Error("Workspace không tồn tại.");
      }
    } else {
      const preferredWorkspaceId = slugify(normalizeText(staticBundle.activeWorkspaceId, "", 120), "");
      if (preferredWorkspaceId && accountWorkspaceIds.includes(preferredWorkspaceId)) {
        targetWorkspace = getAllWorkspaces().find((item) => item.id === preferredWorkspaceId) || null;
      }
      if (!targetWorkspace && accountWorkspaceIds.length > 0) {
        targetWorkspace = getAllWorkspaces().find((item) => item.id === accountWorkspaceIds[0]) || null;
      }
    }

    if (targetWorkspace) {
      staticBundle.activeWorkspaceId = targetWorkspace.id;
      setActiveWorkspace(targetWorkspace.id);
    } else {
      staticBundle.activeWorkspaceId = "";
    }

    return {
      role: "user",
      token: `local-static-token-${Date.now()}`,
      account: {
        id: normalizeText(account.id, "", 120),
        username: normalizeText(account.username, adminUser, 120)
      },
      workspace: targetWorkspace
        ? {
            id: targetWorkspace.id,
            name: targetWorkspace.name,
            adminUser: normalizeText(account.username, adminUser, 120),
            widgetKey: targetWorkspace.widgetKey
          }
        : null
    };
  }

  function buildWorkspaceRecord(workspaceName, workspaceId) {
    return {
      id: workspaceId,
      name: workspaceName,
      widgetKey: `wk_${workspaceId}`,
      data: {
        config: mergeConfig({
          ...DEFAULT_CONFIG,
          brand: {
            ...DEFAULT_CONFIG.brand,
            name: workspaceName,
            launcherLabel: workspaceName
          }
        }),
        documents: [],
        platformProjects: []
      }
    };
  }

  function assignWorkspaceToAccount(accountId, workspaceId) {
    const normalizedWorkspaceId = slugify(normalizeText(workspaceId, "", 120), "");
    if (!accountId || !normalizedWorkspaceId) {
      return;
    }
    const accounts = getAllAccounts().map((item) => {
      if (normalizeText(item && item.id, "", 120) !== accountId) {
        return item;
      }
      const workspaceIds = normalizeWorkspaceIdList([
        ...(Array.isArray(item.workspaceIds) ? item.workspaceIds : []),
        normalizedWorkspaceId
      ]);
      return {
        ...item,
        workspaceIds
      };
    });
    staticBundle.auth.accounts = accounts;
  }

  async function createWorkspaceForCurrentUser() {
    ensureBundleState();
    if (!currentUserRole) {
      throw new Error("Bạn cần đăng nhập trước khi tạo workspace.");
    }
    if (currentUserRole !== "user") {
      throw new Error("Super Admin không có quyền tạo workspace.");
    }
    const workspaceName = normalizeText(els.workspaceCreateName ? els.workspaceCreateName.value : "", "", 160);
    const workspaceId = slugify(
      normalizeText(els.workspaceCreateId ? els.workspaceCreateId.value : "", workspaceName, 120),
      ""
    );
    if (!workspaceName || !workspaceId) {
      throw new Error("Bạn cần nhập tên workspace và mã workspace.");
    }
    const hasWorkspace = getAllWorkspaces().some((item) => item && item.id === workspaceId);
    if (hasWorkspace) {
      throw new Error("Mã workspace đã tồn tại. Hãy chọn mã khác.");
    }

    const nextWorkspaces = [...getAllWorkspaces(), buildWorkspaceRecord(workspaceName, workspaceId)];
    staticBundle.workspaces = nextWorkspaces;
    const account = getCurrentAccount();
    if (!account) {
      throw new Error("Không tìm thấy tài khoản đăng nhập hiện tại.");
    }
    assignWorkspaceToAccount(normalizeText(account.id, "", 120), workspaceId);
    staticBundle.activeWorkspaceId = workspaceId;
    setActiveWorkspace(workspaceId);
    persistBundleState();
    saveSession({
      role: currentUserRole,
      workspaceId,
      adminUser: normalizeText(currentWorkspace && currentWorkspace.adminUser ? currentWorkspace.adminUser : "", "", 120),
      accountId: normalizeText(currentAccountId, "", 120),
      username: normalizeText(currentAccountUsername, "", 120)
    });
    return currentWorkspace;
  }

  async function openWorkspaceForCurrentUser(workspaceId) {
    ensureBundleState();
    if (currentUserRole !== "user") {
      throw new Error("Super Admin không có quyền mở workspace.");
    }
    const normalizedWorkspaceId = slugify(normalizeText(workspaceId, "", 120), "");
    if (!normalizedWorkspaceId) {
      throw new Error("Bạn chưa chọn workspace.");
    }
    if (currentUserRole === "user") {
      const account = getCurrentAccount();
      if (!account || !accountHasWorkspace(account, normalizedWorkspaceId)) {
        throw new Error("Bạn không có quyền vào workspace này.");
      }
    }
    setActiveWorkspace(normalizedWorkspaceId);
    persistBundleState();
    saveSession({
      role: currentUserRole,
      workspaceId: normalizedWorkspaceId,
      adminUser: normalizeText(currentWorkspace && currentWorkspace.adminUser ? currentWorkspace.adminUser : "", "", 120),
      accountId: normalizeText(currentAccountId, "", 120),
      username: normalizeText(currentAccountUsername, "", 120)
    });
    return currentWorkspace;
  }

  async function restoreWorkspaceSession() {
    ensureBundleState();
    const session = readSession();
    if (!session || typeof session !== "object") {
      return false;
    }
    const role = normalizeText(session.role, "user", 30).toLowerCase() === "superadmin" ? "superadmin" : "user";
    const workspaceId = slugify(normalizeText(session.workspaceId, "", 120), "");
    const workspace = getAllWorkspaces().find((item) => item.id === workspaceId) || null;

    if (role === "user") {
      const accountId = normalizeText(session.accountId, "", 120);
      const username = normalizeText(session.username, "", 120);
      const account = findAccountById(accountId)
        || findAccountByUsername(username)
        || (workspace
          ? getAllAccounts().find((item) => accountHasWorkspace(item, workspace.id)) || null
          : null);
      if (!account) {
        return false;
      }
      if (isAccountLocked(account)) {
        clearSession();
        return false;
      }
      const allowedWorkspaceIds = normalizeAccountWorkspaceIds(account.workspaceIds);
      let targetWorkspace = null;
      if (workspace && allowedWorkspaceIds.includes(workspace.id)) {
        targetWorkspace = workspace;
      } else if (allowedWorkspaceIds.length > 0) {
        targetWorkspace = getAllWorkspaces().find((item) => item.id === allowedWorkspaceIds[0]) || null;
      }
      applyAuthSessionPayload({
        role: "user",
        account: {
          id: normalizeText(account.id, "", 120),
          username: normalizeText(account.username, "", 120)
        },
        workspace: targetWorkspace
          ? {
              id: targetWorkspace.id,
              name: targetWorkspace.name,
              adminUser: normalizeText(account.username, "", 120),
              widgetKey: targetWorkspace.widgetKey
            }
          : null
      });
      return true;
    }
    applyAuthSessionPayload({
      role: "superadmin",
      workspace: null
    });
    return true;
  }

  async function logoutWorkspaceAccount() {
    authToken = "";
    currentWorkspace = null;
    currentUserRole = "";
    currentAccountId = "";
    currentAccountUsername = "";
    writeStoredAuthToken("");
    clearSession();
    setActiveAuthTab("login");
    setWorkspaceDependentVisible(false);
    setAuthenticatedView(false);
    setAuthStatus("Bạn đã đăng xuất.", "muted");
  }

  function parseLines(text) {
    if (typeof text !== "string") {
      return [];
    }
    return text
      .split(/\r?\n/g)
      .map((item) => item.trim())
      .filter(Boolean);
  }

  function parseCsv(text) {
    if (typeof text !== "string") {
      return [];
    }
    return text
      .split(",")
      .map((item) => item.trim())
      .filter(Boolean);
  }

  function parseFlexibleList(text) {
    if (typeof text !== "string") {
      return [];
    }
    return text
      .split(/[\n,|]+/g)
      .map((item) => item.trim())
      .filter(Boolean);
  }

  function parsePipeList(text) {
    if (typeof text !== "string") {
      return [];
    }
    return text
      .split("|")
      .map((item) => item.trim())
      .filter(Boolean);
  }

  function splitCsvLine(line) {
    const values = [];
    let current = "";
    let inQuotes = false;
    for (let i = 0; i < line.length; i += 1) {
      const ch = line[i];
      if (ch === "\"") {
        const next = line[i + 1];
        if (inQuotes && next === "\"") {
          current += "\"";
          i += 1;
          continue;
        }
        inQuotes = !inQuotes;
        continue;
      }
      if (ch === "," && !inQuotes) {
        values.push(current);
        current = "";
        continue;
      }
      current += ch;
    }
    values.push(current);
    return values.map((item) => item.trim());
  }

  function shorten(text, max) {
    if (typeof text !== "string") {
      return "";
    }
    if (text.length <= max) {
      return text;
    }
    return `${text.slice(0, max)}...`;
  }

  function escapeHtml(text) {
    return (text || "")
      .toString()
      .replace(/&/g, "&amp;")
      .replace(/</g, "&lt;")
      .replace(/>/g, "&gt;")
      .replace(/"/g, "&quot;")
      .replace(/'/g, "&#39;");
  }

  function createKbRow(row) {
    const wrapper = document.createElement("div");
    wrapper.className = "kb-row";
    wrapper.innerHTML = `
      <div class="kb-row-grid">
        <label>Từ khóa (phân tách dấu phẩy)
          <textarea class="kb-keywords" rows="4"></textarea>
        </label>
        <label>Câu trả lời
          <textarea class="kb-answer" rows="4"></textarea>
        </label>
        <label>Nút gợi ý nhanh (phân tách dấu phẩy)
          <textarea class="kb-replies" rows="4"></textarea>
        </label>
        <button type="button" class="remove-kb">Xóa</button>
      </div>
    `;
    const keywordField = wrapper.querySelector(".kb-keywords");
    const answerField = wrapper.querySelector(".kb-answer");
    const repliesField = wrapper.querySelector(".kb-replies");
    const removeButton = wrapper.querySelector(".remove-kb");

    keywordField.value = Array.isArray(row.keywords) ? row.keywords.join(", ") : "";
    answerField.value = row.answer || "";
    repliesField.value = Array.isArray(row.quickReplies) ? row.quickReplies.join(", ") : "";
    removeButton.addEventListener("click", () => wrapper.remove());

    return wrapper;
  }

  function parseKbFromJsonText(text) {
    const parsed = JSON.parse(text);
    const source = Array.isArray(parsed)
      ? parsed
      : parsed && Array.isArray(parsed.knowledgeBase)
        ? parsed.knowledgeBase
        : [];
    const rows = [];
    source.forEach((item, index) => {
      const row = item && typeof item === "object" ? item : {};
      const answer = normalizeText(row.answer, "", 2000);
      if (!answer) {
        return;
      }
      const keywords = Array.isArray(row.keywords)
        ? row.keywords.map((x) => normalizeText(x, "", 100)).filter(Boolean)
        : parsePipeList(row.keywords || "");
      const quickReplies = Array.isArray(row.quickReplies)
        ? row.quickReplies.map((x) => normalizeText(x, "", 100)).filter(Boolean)
        : parsePipeList(row.quickReplies || "");
      rows.push({
        id: normalizeText(row.id, `kb_${index + 1}`, 80),
        keywords,
        answer,
        quickReplies
      });
    });
    return rows;
  }

  function parseKbFromCsvText(text) {
    const lines = (text || "")
      .split(/\r?\n/g)
      .map((line) => line.trim())
      .filter(Boolean);
    if (lines.length === 0) {
      return [];
    }
    const headers = splitCsvLine(lines[0]).map((h) => h.trim().toLowerCase());
    const idxId = headers.indexOf("id");
    const idxKeywords = headers.indexOf("keywords");
    const idxAnswer = headers.indexOf("answer");
    const idxQuick = headers.indexOf("quickreplies");
    if (idxKeywords < 0 || idxAnswer < 0) {
      throw new Error("File CSV thiếu cột bắt buộc: keywords, answer.");
    }
    const rows = [];
    for (let i = 1; i < lines.length; i += 1) {
      const columns = splitCsvLine(lines[i]);
      const answer = normalizeText(columns[idxAnswer] || "", "", 2000);
      if (!answer) {
        continue;
      }
      const rawKeywords = columns[idxKeywords] || "";
      const rawQuick = idxQuick >= 0 ? columns[idxQuick] || "" : "";
      const id = idxId >= 0 ? normalizeText(columns[idxId] || "", "", 80) : "";
      rows.push({
        id: id || `kb_${i}`,
        keywords: parsePipeList(rawKeywords),
        answer,
        quickReplies: parsePipeList(rawQuick)
      });
    }
    return rows;
  }

  function parseDocumentsFromCsvText(text) {
    const lines = (text || "")
      .split(/\r?\n/g)
      .map((line) => line.trim())
      .filter(Boolean);
    if (lines.length === 0) {
      return [];
    }
    const headers = splitCsvLine(lines[0]).map((h) => h.trim().toLowerCase());
    const idxId = headers.indexOf("id");
    const idxTitle = headers.indexOf("title");
    const idxTags = headers.indexOf("tags");
    const idxText = headers.indexOf("text");
    const idxSourceUrls = headers.indexOf("sourceurls");
    if (idxTitle < 0 || idxText < 0) {
      throw new Error("File CSV tài liệu thiếu cột bắt buộc: title, text.");
    }
    const rows = [];
    for (let i = 1; i < lines.length; i += 1) {
      const columns = splitCsvLine(lines[i]);
      const textValue = normalizeText(columns[idxText] || "", "", 120000);
      if (!textValue) {
        continue;
      }
      const titleValue = normalizeText(columns[idxTitle] || "", `Tài liệu ${i}`, 180);
      const tagValue = idxTags >= 0 ? normalizeText(columns[idxTags] || "", "", 500) : "";
      const sourceValue = idxSourceUrls >= 0 ? normalizeText(columns[idxSourceUrls] || "", "", 2000) : "";
      const sourceUrls = parseFlexibleList(sourceValue)
        .map((url) => normalizeUrlInput(url))
        .filter(Boolean)
        .slice(0, 10);
      const idValue = idxId >= 0 ? normalizeText(columns[idxId] || "", "", 160) : "";
      rows.push({
        id: idValue || "",
        title: titleValue,
        text: textValue,
        tags: parseFlexibleList(tagValue).slice(0, 20),
        sourceUrls
      });
    }
    return rows;
  }

  function parseDocumentsFromJsonText(text) {
    const parsed = JSON.parse(text);
    const source = Array.isArray(parsed)
      ? parsed
      : parsed && Array.isArray(parsed.documents)
        ? parsed.documents
        : [];
    const rows = [];
    source.forEach((item, index) => {
      const normalized = normalizeDocumentRecord(item, index);
      if (!normalized) {
        return;
      }
      rows.push({
        id: normalizeText(normalized.id, "", 160),
        title: normalizeText(normalized.title, `Tài liệu ${index + 1}`, 180),
        text: normalizeText(normalized.text, "", 120000),
        tags: Array.isArray(normalized.tags) ? normalized.tags.slice(0, 20) : [],
        sourceUrls: Array.isArray(normalized.sourceUrls) ? normalized.sourceUrls.slice(0, 10) : []
      });
    });
    return rows;
  }

  function buildKnowledgeBaseCsvText(rows) {
    const header = buildCsvLine(["id", "keywords", "answer", "quickReplies"]);
    const body = (Array.isArray(rows) ? rows : []).map((row, index) => {
      const source = row && typeof row === "object" ? row : {};
      const id = normalizeText(source.id, `kb_${index + 1}`, 80);
      const keywords = Array.isArray(source.keywords) ? source.keywords.join("|") : "";
      const answer = normalizeText(source.answer, "", 2000);
      const quickReplies = Array.isArray(source.quickReplies) ? source.quickReplies.join("|") : "";
      return buildCsvLine([id, keywords, answer, quickReplies]);
    });
    return [header, ...body].join("\n");
  }

  function buildDocumentsCsvText(items) {
    const header = buildCsvLine(["id", "title", "tags", "text", "sourceUrls"]);
    const body = (Array.isArray(items) ? items : []).map((row, index) => {
      const source = row && typeof row === "object" ? row : {};
      const id = normalizeText(source.id, `doc_${index + 1}`, 160);
      const title = normalizeText(source.title, `Tài liệu ${index + 1}`, 180);
      const tags = Array.isArray(source.tags) ? source.tags.join("|") : "";
      const textValue = normalizeText(source.text, "", 120000);
      const sourceUrls = Array.isArray(source.sourceUrls) ? source.sourceUrls.join("|") : "";
      return buildCsvLine([id, title, tags, textValue, sourceUrls]);
    });
    return [header, ...body].join("\n");
  }

  function renderLogoPreview(dataUrl) {
    els.logoPreview.innerHTML = "";
    if (!dataUrl) {
      return;
    }
    const image = document.createElement("img");
    image.src = dataUrl;
    image.alt = "Logo khung chat";
    els.logoPreview.appendChild(image);
  }

  function renderKbList(rows) {
    els.kbList.innerHTML = "";
    const items = Array.isArray(rows) ? rows : [];
    if (items.length === 0) {
      els.kbList.appendChild(createKbRow({ keywords: [], answer: "", quickReplies: [] }));
      return;
    }
    items.forEach((row) => {
      els.kbList.appendChild(createKbRow(row));
    });
  }

  function mergeConfig(raw) {
    const source = raw && typeof raw === "object" ? raw : {};
    return {
      brand: {
        ...DEFAULT_CONFIG.brand,
        ...(source.brand || {})
      },
      theme: {
        ...DEFAULT_CONFIG.theme,
        ...(source.theme || {})
      },
      layout: {
        ...DEFAULT_CONFIG.layout,
        ...(source.layout || {})
      },
      behavior: {
        ...DEFAULT_CONFIG.behavior,
        ...(source.behavior || {})
      },
      quickStartButtons: Array.isArray(source.quickStartButtons)
        ? source.quickStartButtons.slice(0, 12)
        : DEFAULT_CONFIG.quickStartButtons.slice(),
      knowledgeBase: Array.isArray(source.knowledgeBase) ? source.knowledgeBase.slice() : [],
      fallbackAnswer: normalizeText(source.fallbackAnswer, DEFAULT_CONFIG.fallbackAnswer, 2000)
    };
  }

  function cloneJson(value) {
    return JSON.parse(JSON.stringify(value));
  }

  function createWorkspaceData(raw) {
    const source = raw && typeof raw === "object" ? raw : {};
    return {
      config: mergeConfig(source.config || source),
      documents: Array.isArray(source.documents) ? source.documents : [],
      platformProjects: Array.isArray(source.platformProjects)
        ? source.platformProjects
        : source.platform && Array.isArray(source.platform.projects)
          ? source.platform.projects
          : []
    };
  }

  function buildDefaultBundle() {
    const defaultWorkspaceId = "default-site";
    const data = createWorkspaceData(DEFAULT_CONFIG);
    return {
      schemaVersion: 2,
      updatedAt: "",
      auth: {
        superAdmins: [
          {
            username: DEFAULT_SUPER_ADMIN.username,
            password: DEFAULT_SUPER_ADMIN.password
          }
        ],
        accounts: [
          {
            id: "acc_admin",
            username: "admin",
            displayName: "Quản trị mặc định",
            password: "123456",
            workspaceIds: [defaultWorkspaceId],
            locked: false,
            lockedAt: ""
          }
        ]
      },
      activeWorkspaceId: defaultWorkspaceId,
      workspaces: [
        {
          id: defaultWorkspaceId,
          name: "Workspace mặc định",
          widgetKey: `wk_${defaultWorkspaceId}`,
          data: {
            config: data.config,
            documents: [],
            platformProjects: []
          }
        }
      ],
      config: data.config,
      documents: [],
      platformProjects: []
    };
  }

  function normalizeDocId(value, fallback) {
    const normalized = normalizeText(value, fallback, 160)
      .toLowerCase()
      .replace(/[^\w-]+/g, "-")
      .replace(/-+/g, "-")
      .replace(/^-+|-+$/g, "");
    return normalized || fallback;
  }

  function normalizeDocumentRecord(raw, index) {
    const source = raw && typeof raw === "object" ? raw : {};
    const now = new Date().toISOString();
    const title = normalizeText(source.title, `Tài liệu ${index + 1}`, 180);
    const text = normalizeText(source.text, "", 120000);
    if (!text) {
      return null;
    }
    const tags = Array.isArray(source.tags) ? source.tags.map((item) => normalizeText(item, "", 40)).filter(Boolean) : [];
    const sourceUrlsRaw = Array.isArray(source.sourceUrls)
      ? source.sourceUrls
      : source.meta && Array.isArray(source.meta.sourceUrls)
        ? source.meta.sourceUrls
        : [];
    const sourceUrls = sourceUrlsRaw
      .map((url) => normalizeUrlInput(url))
      .filter(Boolean)
      .slice(0, 10);
    const idFallback = `doc_${index + 1}_${Date.now()}`;
    const id = normalizeDocId(source.id, idFallback);
    const meta = source.meta && typeof source.meta === "object" ? { ...source.meta } : {};
    if (sourceUrls.length > 0) {
      meta.sourceUrls = sourceUrls.slice();
    }
    return {
      id,
      title,
      text,
      tags: tags.slice(0, 20),
      sourceUrls,
      createdAt: normalizeText(source.createdAt, now, 40),
      updatedAt: normalizeText(source.updatedAt, now, 40),
      length: text.length,
      meta
    };
  }

  function normalizeGuideDraft(raw, index, projectId) {
    const source = raw && typeof raw === "object" ? raw : {};
    const fallbackId = `guide_${projectId}_${index + 1}`;
    const id = normalizeDocId(source.id, fallbackId);
    const title = normalizeText(source.title, `Hướng dẫn ${index + 1}`, 180);
    const answer = normalizeText(source.answer, "", 3000);
    if (!answer) {
      return null;
    }
    const intentKeywords = parseFlexibleList(Array.isArray(source.intentKeywords)
      ? source.intentKeywords.join(", ")
      : source.intentKeywords || source.keywords || "").slice(0, 12);
    const quickReplies = parseFlexibleList(Array.isArray(source.quickReplies)
      ? source.quickReplies.join(", ")
      : source.quickReplies || "").slice(0, 4);
    const sourceUrls = Array.isArray(source.sourceUrls)
      ? source.sourceUrls.map((url) => normalizeUrlInput(url)).filter(Boolean).slice(0, 10)
      : [];
    return {
      id,
      title,
      answer,
      intentKeywords,
      quickReplies: quickReplies.length > 0 ? quickReplies : intentKeywords.slice(0, 3),
      sourceUrls,
      reviewed: Boolean(source.reviewed)
    };
  }

  function normalizeGuideDrafts(input, projectId) {
    const rows = Array.isArray(input) ? input : [];
    const normalized = [];
    const seen = new Set();
    for (let i = 0; i < rows.length; i += 1) {
      const item = normalizeGuideDraft(rows[i], i, projectId);
      if (!item || seen.has(item.id)) {
        continue;
      }
      seen.add(item.id);
      normalized.push(item);
      if (normalized.length >= 200) {
        break;
      }
    }
    return normalized;
  }

  function normalizePlatformProject(raw, index) {
    const source = raw && typeof raw === "object" ? raw : {};
    const id = slugify(normalizeText(source.id, `site-${index + 1}`, 120), `site-${index + 1}`);
    const settings = source.settings && typeof source.settings === "object" ? source.settings : {};
    const crawl = source.crawl && typeof source.crawl === "object" ? source.crawl : {};
    return {
      id,
      name: normalizeText(source.name, id, 160),
      baseUrl: normalizeUrlInput(source.baseUrl || ""),
      settings: {
        maxPages: clamp(settings.maxPages, 1, 200, 40),
        maxDepth: clamp(settings.maxDepth, 0, 6, 3),
        timeoutMs: clamp(settings.timeoutMs, 3000, 30000, 10000),
        includePaths: parseLines(Array.isArray(settings.includePaths) ? settings.includePaths.join("\n") : settings.includePaths || ""),
        excludePaths: parseLines(Array.isArray(settings.excludePaths) ? settings.excludePaths.join("\n") : settings.excludePaths || ""),
        requestHeaders: settings.requestHeaders && typeof settings.requestHeaders === "object" ? settings.requestHeaders : {}
      },
      crawl: {
        status: normalizeText(crawl.status, "idle", 20),
        startedAt: normalizeText(crawl.startedAt, "", 40),
        endedAt: normalizeText(crawl.endedAt, "", 40),
        summary: crawl.summary && typeof crawl.summary === "object" ? crawl.summary : {},
        guides: normalizeGuideDrafts(crawl.guides, id),
        pages: Array.isArray(crawl.pages) ? crawl.pages.slice(0, 500) : [],
        errors: Array.isArray(crawl.errors) ? crawl.errors.slice(0, 200) : []
      }
    };
  }

  function normalizeBundle(raw) {
    const source = raw && typeof raw === "object" ? raw : {};
    const legacyConfig = source.config && typeof source.config === "object" ? source.config : source;
    const legacyDocs = Array.isArray(source.documents)
      ? source.documents
      : Array.isArray(legacyConfig.documents)
        ? legacyConfig.documents
        : [];
    const legacyProjects = Array.isArray(source.platformProjects)
      ? source.platformProjects
      : source.platform && Array.isArray(source.platform.projects)
        ? source.platform.projects
        : [];

    const authSource = source.auth && typeof source.auth === "object" ? source.auth : {};
    const superAdminsRaw = Array.isArray(authSource.superAdmins) ? authSource.superAdmins : [];
    const accountsRaw = Array.isArray(authSource.accounts) ? authSource.accounts : [];
    const workspacesRaw = Array.isArray(source.workspaces) ? source.workspaces : [];

    const normalizedSuperAdmins = superAdminsRaw
      .map((item) => ({
        username: normalizeText(item && item.username, "", 80),
        password: normalizeText(item && item.password, "", 200)
      }))
      .filter((item) => item.username && item.password);

    const normalizedWorkspaces = workspacesRaw
      .map((item, index) => {
        const id = slugify(normalizeText(item && item.id, `workspace-${index + 1}`, 120), `workspace-${index + 1}`);
        const workspaceData = createWorkspaceData(item && item.data ? item.data : item);
        const docs = [];
        const docsSource = Array.isArray(workspaceData.documents) ? workspaceData.documents : [];
        for (let i = 0; i < docsSource.length; i += 1) {
          const doc = normalizeDocumentRecord(docsSource[i], i);
          if (doc) {
            docs.push(doc);
          }
        }
        return {
          id,
          name: normalizeText(item && item.name, id, 160),
          widgetKey: normalizeText(item && item.widgetKey, `wk_${id}`, 120),
          data: {
            config: mergeConfig(workspaceData.config),
            documents: docs,
            platformProjects: (Array.isArray(workspaceData.platformProjects) ? workspaceData.platformProjects : [])
              .map((project, projectIndex) => normalizePlatformProject(project, projectIndex))
          }
        };
      });

    if (normalizedWorkspaces.length === 0) {
      const defaultWorkspaceId = "default-site";
      const docs = [];
      for (let i = 0; i < legacyDocs.length; i += 1) {
        const doc = normalizeDocumentRecord(legacyDocs[i], i);
        if (doc) {
          docs.push(doc);
        }
      }
      normalizedWorkspaces.push({
        id: defaultWorkspaceId,
        name: "Workspace mặc định",
        widgetKey: `wk_${defaultWorkspaceId}`,
        data: {
          config: mergeConfig(legacyConfig),
          documents: docs,
          platformProjects: legacyProjects.map((project, index) => normalizePlatformProject(project, index))
        }
      });
    }

    const workspaceIdSet = new Set(normalizedWorkspaces.map((item) => item.id));
    const accountByUsername = new Map();
    const usedAccountIds = new Set();

    const makeUniqueAccountId = (preferred, username) => {
      const fallbackBase = `acc_${slugify(username || "user", "user")}`;
      const base = normalizeText(preferred, "", 120) || fallbackBase;
      const safeBase = slugify(base, fallbackBase);
      let candidate = safeBase;
      let suffix = 2;
      while (usedAccountIds.has(candidate)) {
        candidate = `${safeBase}_${suffix}`;
        suffix += 1;
      }
      usedAccountIds.add(candidate);
      return candidate;
    };

    accountsRaw.forEach((item) => {
      const sourceAccount = item && typeof item === "object" ? item : {};
      const username = normalizeText(sourceAccount.username || sourceAccount.adminUser, "", 120);
      const password = normalizeText(sourceAccount.password, "", 200);
      if (!username || !password) {
        return;
      }
      const displayName = normalizeText(sourceAccount.displayName, username, 120);
      const locked = sourceAccount.locked === true;
      const lockedAt = normalizeText(sourceAccount.lockedAt, "", 80);
      const explicitWorkspaceIds = Array.isArray(sourceAccount.workspaceIds) ? sourceAccount.workspaceIds : [];
      const legacyWorkspaceId = normalizeText(sourceAccount.workspaceId, "", 120);
      const workspaceIds = normalizeWorkspaceIdList([
        ...explicitWorkspaceIds,
        ...(legacyWorkspaceId ? [legacyWorkspaceId] : [])
      ]).filter((id) => workspaceIdSet.has(id));
      const usernameKey = username.toLowerCase();
      const existing = accountByUsername.get(usernameKey);
      if (existing) {
        if (existing.password === password) {
          existing.workspaceIds = normalizeWorkspaceIdList([...(existing.workspaceIds || []), ...workspaceIds]);
          if (locked) {
            existing.locked = true;
            existing.lockedAt = lockedAt || existing.lockedAt || "";
          }
        }
        return;
      }
      const accountId = makeUniqueAccountId(sourceAccount.id, username);
      accountByUsername.set(usernameKey, {
        id: accountId,
        username,
        displayName,
        password,
        workspaceIds,
        locked,
        lockedAt: locked ? lockedAt : ""
      });
    });

    const normalizedAccounts = Array.from(accountByUsername.values())
      .map((item) => ({
        ...item,
        locked: item.locked === true,
        lockedAt: item.locked === true ? normalizeText(item.lockedAt, "", 80) : "",
        workspaceIds: normalizeWorkspaceIdList(item.workspaceIds).filter((id) => workspaceIdSet.has(id))
      }))
      .filter((item) => item.username && item.password);

    if (normalizedAccounts.length === 0) {
      const fallbackWorkspaceId = normalizedWorkspaces[0] ? normalizedWorkspaces[0].id : "default-site";
      normalizedAccounts.push({
        id: "acc_admin",
        username: "admin",
        displayName: "Quản trị mặc định",
        password: "123456",
        workspaceIds: fallbackWorkspaceId ? [fallbackWorkspaceId] : [],
        locked: false,
        lockedAt: ""
      });
    }

    if (normalizedSuperAdmins.length === 0) {
      normalizedSuperAdmins.push({
        username: DEFAULT_SUPER_ADMIN.username,
        password: DEFAULT_SUPER_ADMIN.password
      });
    }

    const activeWorkspaceIdRaw = slugify(normalizeText(source.activeWorkspaceId, "", 120), "");
    const activeWorkspaceId = normalizedWorkspaces.some((item) => item.id === activeWorkspaceIdRaw)
      ? activeWorkspaceIdRaw
      : (normalizedWorkspaces[0] ? normalizedWorkspaces[0].id : "");
    const activeWorkspace = normalizedWorkspaces.find((item) => item.id === activeWorkspaceId) || normalizedWorkspaces[0] || null;

    return {
      schemaVersion: 2,
      updatedAt: normalizeText(source.updatedAt, "", 40),
      auth: {
        superAdmins: normalizedSuperAdmins,
        accounts: normalizedAccounts
      },
      activeWorkspaceId,
      workspaces: normalizedWorkspaces,
      config: mergeConfig(activeWorkspace && activeWorkspace.data ? activeWorkspace.data.config : DEFAULT_CONFIG),
      documents: cloneJson(activeWorkspace && activeWorkspace.data && Array.isArray(activeWorkspace.data.documents) ? activeWorkspace.data.documents : []),
      platformProjects: cloneJson(
        activeWorkspace && activeWorkspace.data && Array.isArray(activeWorkspace.data.platformProjects)
          ? activeWorkspace.data.platformProjects
          : []
      )
    };
  }

  function createBundleForExport() {
    persistTopLevelIntoActiveWorkspace();
    const fallbackWorkspaceId = normalizeText(
      (Array.isArray(staticBundle && staticBundle.workspaces) && staticBundle.workspaces[0] && staticBundle.workspaces[0].id)
        || "default-site",
      "default-site",
      120
    );
    return {
      schemaVersion: 2,
      updatedAt: new Date().toISOString(),
      auth: cloneJson(staticBundle && staticBundle.auth ? staticBundle.auth : buildDefaultBundle().auth),
      activeWorkspaceId: normalizeText(staticBundle && staticBundle.activeWorkspaceId, fallbackWorkspaceId, 120),
      workspaces: cloneJson(Array.isArray(staticBundle && staticBundle.workspaces) ? staticBundle.workspaces : [])
    };
  }

  function writeBundleCache(bundle) {
    try {
      localStorage.setItem(STATIC_BUNDLE_CACHE_KEY, JSON.stringify(bundle));
    } catch (_) {}
  }

  function readBundleCache() {
    try {
      const raw = localStorage.getItem(STATIC_BUNDLE_CACHE_KEY);
      if (!raw) {
        return null;
      }
      return normalizeBundle(parseJsonSafely(raw) || {});
    } catch (_) {
      return null;
    }
  }

  function toBundleTimestamp(value) {
    const text = normalizeText(value, "", 80);
    if (!text) {
      return 0;
    }
    const time = Date.parse(text);
    return Number.isFinite(time) ? time : 0;
  }

  function shouldPreferCacheBundle(cacheBundle, hostBundle) {
    if (!cacheBundle || !hostBundle) {
      return false;
    }
    const cacheTime = toBundleTimestamp(cacheBundle.updatedAt);
    const hostTime = toBundleTimestamp(hostBundle.updatedAt);
    if (cacheTime > hostTime) {
      return true;
    }
    if (cacheTime === hostTime) {
      return false;
    }
    return false;
  }

  function persistTopLevelIntoActiveWorkspace() {
    ensureBundleState();
    const workspaceId = normalizeText(staticBundle.activeWorkspaceId, "", 120);
    if (!workspaceId) {
      return;
    }
    const list = Array.isArray(staticBundle.workspaces) ? staticBundle.workspaces : [];
    const index = list.findIndex((item) => item && item.id === workspaceId);
    if (index < 0) {
      return;
    }
    const workspace = list[index];
    const nextWorkspace = {
      ...workspace,
      data: {
        config: mergeConfig(staticBundle.config),
        documents: cloneJson(Array.isArray(staticBundle.documents) ? staticBundle.documents : []),
        platformProjects: cloneJson(Array.isArray(staticBundle.platformProjects) ? staticBundle.platformProjects : [])
      }
    };
    list[index] = nextWorkspace;
    staticBundle.workspaces = list;
  }

  function setActiveWorkspace(workspaceId) {
    ensureBundleState();
    persistTopLevelIntoActiveWorkspace();
    const normalizedId = slugify(workspaceId, "");
    if (!normalizedId) {
      throw new Error("Bạn chưa chọn workspace.");
    }
    if (currentUserRole === "user") {
      const account = getCurrentAccount();
      if (!account || !accountHasWorkspace(account, normalizedId)) {
        throw new Error("Bạn không có quyền vào workspace này.");
      }
    }
    const workspace = (Array.isArray(staticBundle.workspaces) ? staticBundle.workspaces : [])
      .find((item) => item && item.id === normalizedId);
    if (!workspace) {
      throw new Error("Không tìm thấy workspace.");
    }
    staticBundle.activeWorkspaceId = workspace.id;
    staticBundle.config = mergeConfig(workspace.data && workspace.data.config ? workspace.data.config : DEFAULT_CONFIG);
    staticBundle.documents = cloneJson(Array.isArray(workspace.data && workspace.data.documents) ? workspace.data.documents : []);
    staticBundle.platformProjects = cloneJson(Array.isArray(workspace.data && workspace.data.platformProjects) ? workspace.data.platformProjects : []);
    const account = getCurrentAccount();
    const defaultAdminUser = currentUserRole === "superadmin"
      ? normalizeText(currentWorkspace && currentWorkspace.adminUser ? currentWorkspace.adminUser : "", DEFAULT_SUPER_ADMIN.username, 120)
      : normalizeText(
          currentAccountUsername || normalizeText(account && account.username, "", 120),
          "admin",
          120
        );
    currentWorkspace = {
      id: workspace.id,
      name: workspace.name,
      widgetKey: workspace.widgetKey,
      adminUser: defaultAdminUser
    };
    updateWorkspaceBadge();
  }

  function saveSession(payload) {
    try {
      localStorage.setItem(STATIC_SESSION_KEY, JSON.stringify(payload || {}));
    } catch (_) {}
  }

  function readSession() {
    try {
      const raw = localStorage.getItem(STATIC_SESSION_KEY);
      if (!raw) {
        return null;
      }
      const parsed = parseJsonSafely(raw);
      return parsed && typeof parsed === "object" ? parsed : null;
    } catch (_) {
      return null;
    }
  }

  function clearSession() {
    try {
      localStorage.removeItem(STATIC_SESSION_KEY);
    } catch (_) {}
  }

  function renderWorkspaceToolsPanel() {
    if (!els.workspaceTools) {
      return;
    }
    const accessibleWorkspaces = getAccessibleWorkspacesForCurrentUser();
    if (els.workspaceSwitchSelect) {
      els.workspaceSwitchSelect.innerHTML = "";
      accessibleWorkspaces.forEach((workspace) => {
        const option = document.createElement("option");
        option.value = workspace.id;
        option.textContent = `${workspace.name} (${workspace.id})`;
        if (currentWorkspace && workspace.id === currentWorkspace.id) {
          option.selected = true;
        }
        els.workspaceSwitchSelect.appendChild(option);
      });
      if (accessibleWorkspaces.length === 0) {
        const emptyOption = document.createElement("option");
        emptyOption.value = "";
        emptyOption.textContent = "Chưa có workspace";
        els.workspaceSwitchSelect.appendChild(emptyOption);
      }
      els.workspaceSwitchSelect.disabled = accessibleWorkspaces.length === 0;
    }
    if (els.workspaceSwitchSubmit) {
      els.workspaceSwitchSubmit.disabled = accessibleWorkspaces.length === 0;
    }
    if (els.workspaceToolsSummary) {
      if (accessibleWorkspaces.length > 0) {
        els.workspaceToolsSummary.textContent =
          `Tài khoản ${normalizeText(currentAccountUsername, "user", 120)} đang có ${accessibleWorkspaces.length} workspace.`;
      } else {
        els.workspaceToolsSummary.textContent =
          "Tài khoản này chưa có workspace. Hãy tạo workspace đầu tiên để bắt đầu cấu hình widget.";
      }
    }
  }

  function renderSuperAdminAccountsPanel() {
    if (!els.superAdminTools) {
      return;
    }
    if (currentUserRole !== "superadmin") {
      els.superAdminTools.hidden = true;
      return;
    }
    els.superAdminTools.hidden = false;
    const accounts = getAllAccounts();
    const allWorkspaces = getAllWorkspaces();
    if (els.superAccountsList) {
      els.superAccountsList.innerHTML = "";
      if (accounts.length === 0) {
        const empty = document.createElement("p");
        empty.className = "hint";
        empty.textContent = "Chưa có tài khoản thường nào.";
        els.superAccountsList.appendChild(empty);
        return;
      }
      accounts.forEach((account) => {
        const workspaceIds = normalizeAccountWorkspaceIds(account.workspaceIds);
        const locked = isAccountLocked(account);
        const workspaceMeta = workspaceIds.map((workspaceId) => {
          const workspace = allWorkspaces.find((item) => item.id === workspaceId);
          return {
            id: workspaceId,
            name: workspace ? workspace.name : workspaceId
          };
        });
        const workspaceText = workspaceMeta.length > 0
          ? workspaceMeta.map((item) => `${item.name} (${item.id})`).join(" | ")
          : "Chưa gán workspace";
        const row = document.createElement("article");
        row.className = "doc-item";
        row.innerHTML = `
          <div class="doc-item-head">
            <h3 class="doc-item-title">${escapeHtml(account.displayName || account.username || "-")}</h3>
            <div class="doc-item-actions">
              <button
                type="button"
                class="super-toggle-lock ${locked ? "is-unlock" : "is-lock"}"
                data-account-id="${escapeHtml(account.id || "")}"
                data-next-locked="${locked ? "0" : "1"}"
              >
                ${locked ? "Mở khóa" : "Khóa tài khoản"}
              </button>
            </div>
          </div>
          <div class="doc-item-meta">
            <span>Tài khoản: ${escapeHtml(account.username || "-")}</span>
            <span>Số workspace: ${workspaceMeta.length}</span>
            <span class="account-state ${locked ? "is-locked" : "is-active"}">${locked ? "Đã khóa" : "Đang hoạt động"}</span>
            <span>${escapeHtml(workspaceText)}</span>
          </div>
        `;
        els.superAccountsList.appendChild(row);
      });
    }
  }

  function syncStateFromBundle() {
    if (!staticBundle) {
      staticBundle = buildDefaultBundle();
    }
    const workspaceId = normalizeText(staticBundle.activeWorkspaceId, "", 120)
      || normalizeText((staticBundle.workspaces && staticBundle.workspaces[0] && staticBundle.workspaces[0].id) || "", "", 120);
    if (workspaceId) {
      try {
        setActiveWorkspace(workspaceId);
      } catch (_) {
        currentWorkspace = null;
        staticBundle.activeWorkspaceId = "";
      }
    } else {
      currentWorkspace = null;
      staticBundle.activeWorkspaceId = "";
    }
    if (currentWorkspace && currentWorkspace.id) {
      renderConfig(staticBundle.config);
      renderDocuments(staticBundle.documents);
      renderPlatformProject(null);
      setWorkspaceDependentVisible(true);
    } else {
      setWorkspaceDependentVisible(false);
    }
    renderWorkspaceToolsPanel();
    renderSuperAdminAccountsPanel();
    if (els.platformProjectId && !normalizeText(els.platformProjectId.value, "", 120)) {
      els.platformProjectId.value = "default-site";
    }
  }

  function renderConfig(config) {
    const cfg = mergeConfig(config);
    els.brandName.value = cfg.brand.name;
    els.agentName.value = cfg.brand.agentName;
    els.launcherLabel.value = cfg.brand.launcherLabel;
    els.welcomeTitle.value = cfg.brand.welcomeTitle;
    els.welcomeMessage.value = cfg.brand.welcomeMessage;
    els.inputPlaceholder.value = cfg.brand.inputPlaceholder;

    els.themePrimary.value = cfg.theme.primaryColor;
    els.themeLauncherBg.value = cfg.theme.launcherBg;
    els.themeTextOnPrimary.value = cfg.theme.textOnPrimary;
    els.themePanelBg.value = cfg.theme.panelBg;
    els.themeBotBubble.value = cfg.theme.botBubbleBg;
    els.themeUserBubble.value = cfg.theme.userBubbleBg;

    els.layoutPosition.value = cfg.layout.position === "left" ? "left" : "right";
    els.layoutBottom.value = String(cfg.layout.bottom);
    els.layoutSide.value = String(cfg.layout.side);
    els.layoutZIndex.value = String(cfg.layout.zIndex);

    els.behaviorShowWelcome.checked = Boolean(cfg.behavior.showWelcomeOnOpen);
    els.behaviorStartOpen.checked = Boolean(cfg.behavior.startOpen);
    els.behaviorTypingDelay.value = String(cfg.behavior.typingDelayMs);
    els.behaviorMaxMessages.value = String(cfg.behavior.maxMessages);

    els.quickStartButtons.value = cfg.quickStartButtons.join("\n");
    els.fallbackAnswer.value = cfg.fallbackAnswer;
    currentLogoDataUrl = cfg.brand.logoDataUrl || "";
    renderLogoPreview(currentLogoDataUrl);
    renderKbList(cfg.knowledgeBase);
    renderEmbedCode();
    syncQuickFormFromMain();
  }

  function getFilteredDocumentItems() {
    const query = foldSearchText(documentsSearchQuery);
    const allItems = Array.isArray(documentItems) ? documentItems : [];
    if (!query) {
      return allItems.slice();
    }
    return allItems.filter((doc) => {
      const tags = Array.isArray(doc && doc.tags) ? doc.tags.join(" ") : "";
      const sourceUrls = Array.isArray(doc && doc.sourceUrls) ? doc.sourceUrls.join(" ") : "";
      const content = `${doc && doc.id ? doc.id : ""} ${doc && doc.title ? doc.title : ""} ${tags} ${sourceUrls} ${
        doc && doc.text ? doc.text : ""
      }`;
      return foldSearchText(content).includes(query);
    });
  }

  function createDocumentListItem(doc) {
    const item = document.createElement("article");
    item.className = "doc-item";
    item.setAttribute("data-id", doc.id || "");
    const tags = Array.isArray(doc.tags) ? doc.tags : [];
    const tagHtml = tags.map((tag) => `<span class="doc-tag">${escapeHtml(tag)}</span>`).join("");
    const tagText = tags.join(", ");
    const snippet = escapeHtml(shorten((doc.text || "").replace(/\s+/g, " ").trim(), 260));
    const size = typeof doc.length === "number" ? doc.length : (doc.text || "").length;

    item.innerHTML = `
      <div class="doc-item-head">
        <h3 class="doc-item-title">${escapeHtml(doc.title || "Không tiêu đề")}</h3>
        <div class="doc-item-actions">
          <button type="button" class="toggle-edit-document" data-id="${escapeHtml(doc.id)}">Sửa</button>
          <button type="button" class="remove-document" data-id="${escapeHtml(doc.id)}">Xóa</button>
        </div>
      </div>
      <div class="doc-item-meta">
        <span>ID: ${escapeHtml(doc.id)}</span>
        <span>${size} ký tự</span>
        <span>Cập nhật: ${escapeHtml(doc.updatedAt || "-")}</span>
        ${tagHtml}
      </div>
      <p class="doc-item-snippet">${snippet}</p>
      <div class="doc-edit" hidden>
        <label>Tiêu đề tài liệu
          <input type="text" class="doc-edit-title" value="${escapeHtml(doc.title || "")}" />
        </label>
        <label>Nhãn (phân tách dấu phẩy)
          <input type="text" class="doc-edit-tags" value="${escapeHtml(tagText)}" />
        </label>
        <label>Nội dung tài liệu
          <textarea class="doc-edit-text" rows="8">${escapeHtml(doc.text || "")}</textarea>
        </label>
        <div class="doc-edit-actions">
          <button type="button" class="save-document-edit" data-id="${escapeHtml(doc.id)}">Lưu chỉnh sửa</button>
          <button type="button" class="cancel-document-edit" data-id="${escapeHtml(doc.id)}">Hủy</button>
        </div>
      </div>
    `;
    return item;
  }

  function renderDocumentListPage() {
    if (els.documentsPageSize) {
      documentsPageSize = normalizeDocumentsPageSize(els.documentsPageSize.value);
    }
    const filteredItems = getFilteredDocumentItems();
    const totalAll = Array.isArray(documentItems) ? documentItems.length : 0;
    const totalFiltered = filteredItems.length;
    const totalPages = Math.max(1, Math.ceil(Math.max(totalFiltered, 1) / documentsPageSize));
    documentsCurrentPage = Math.max(1, Math.min(documentsCurrentPage, totalPages));

    const start = (documentsCurrentPage - 1) * documentsPageSize;
    const pageItems = filteredItems.slice(start, start + documentsPageSize);

    els.documentsList.innerHTML = "";
    if (totalFiltered <= 0) {
      const empty = document.createElement("p");
      empty.className = "hint";
      empty.textContent = documentsSearchQuery
        ? "Không tìm thấy tài liệu khớp từ khóa tìm kiếm."
        : "Chưa có tài liệu nào.";
      els.documentsList.appendChild(empty);
    } else {
      pageItems.forEach((doc) => {
        els.documentsList.appendChild(createDocumentListItem(doc));
      });
    }

    if (els.documentsTotal) {
      els.documentsTotal.textContent = String(totalAll);
    }
    if (els.documentsVisible) {
      els.documentsVisible.textContent = `${pageItems.length}/${totalFiltered}`;
    }
    if (els.documentsPageCurrent) {
      els.documentsPageCurrent.textContent = String(documentsCurrentPage);
    }
    if (els.documentsPageTotal) {
      els.documentsPageTotal.textContent = String(totalPages);
    }
    if (els.documentsPagePrev) {
      els.documentsPagePrev.disabled = documentsCurrentPage <= 1;
    }
    if (els.documentsPageNext) {
      els.documentsPageNext.disabled = documentsCurrentPage >= totalPages;
    }
  }

  function renderDocuments(items) {
    documentItems = Array.isArray(items) ? items.slice() : [];
    renderDocumentListPage();
  }

  function renderPlatformSummary(payload) {
    if (!els.platformSummary) {
      return;
    }
    if (!payload || typeof payload !== "object") {
      els.platformSummary.textContent = "Chưa có dữ liệu quét.";
      return;
    }
    const project = payload.project && typeof payload.project === "object" ? payload.project : payload;
    const settings = project.settings && typeof project.settings === "object" ? project.settings : {};
    const requestHeaders = settings.requestHeaders && typeof settings.requestHeaders === "object"
      ? settings.requestHeaders
      : {};
    const requestHeaderKeys = Object.keys(requestHeaders);
    const crawl = project.crawl && typeof project.crawl === "object" ? project.crawl : {};
    const summary = crawl.summary && typeof crawl.summary === "object" ? crawl.summary : {};
    const review = summary.review && typeof summary.review === "object" ? summary.review : {};
    const pageTypeStats =
      summary.pageTypeStats && typeof summary.pageTypeStats === "object" ? summary.pageTypeStats : {};
    const typeLines = Object.keys(pageTypeStats)
      .slice(0, 12)
      .map((key) => `- ${key}: ${pageTypeStats[key]} trang`);
    const lines = [];
    lines.push(`Website: ${project.name || "-"} (${project.id || "-"})`);
    lines.push(`URL gốc: ${project.baseUrl || "-"}`);
    lines.push(`Trạng thái quét: ${crawl.status || "idle"}`);
    lines.push(`Bắt đầu: ${crawl.startedAt || "-"}`);
    lines.push(`Kết thúc: ${crawl.endedAt || "-"}`);
    lines.push(`Số trang đã quét: ${summary.crawledPages ?? 0}`);
    lines.push(`Hướng dẫn tự sinh: ${summary.generatedGuides ?? 0}`);
    if (typeof review.total === "number") {
      lines.push(`Đã duyệt hướng dẫn: ${review.reviewed ?? 0}/${review.total}`);
    }
    lines.push(`Lỗi: ${summary.errors ?? 0}`);
    if (typeof summary.loginPageRatio === "number") {
      lines.push(`Tỷ lệ trang dạng đăng nhập: ${Math.round(summary.loginPageRatio * 100)}%`);
    }
    if (typeof summary.authBlockedErrors === "number" && summary.authBlockedErrors > 0) {
      lines.push(`Lỗi chặn quyền truy cập (401/403): ${summary.authBlockedErrors}`);
    }
    lines.push(`Đăng nhập phiên khi quét: ${requestHeaderKeys.length > 0 ? "Bật" : "Không"}`);
    if (requestHeaderKeys.length > 0) {
      lines.push(`Header đang dùng: ${requestHeaderKeys.join(", ")}`);
    }
    if (summary.authHint) {
      lines.push(`Gợi ý: ${summary.authHint}`);
    }
    if (typeLines.length > 0) {
      lines.push("Phân loại trang:");
      lines.push(...typeLines);
    }
    els.platformSummary.textContent = lines.join("\n");
  }

  function computeReviewTotals(guides) {
    const items = Array.isArray(guides) ? guides : [];
    const total = items.length;
    let reviewed = 0;
    for (let i = 0; i < items.length; i += 1) {
      if (items[i] && items[i].reviewed) {
        reviewed += 1;
      }
    }
    const pending = Math.max(0, total - reviewed);
    return {
      total,
      reviewed,
      pending
    };
  }

  function updateReviewPublishState(totals, crawlStatus) {
    const total = totals && Number.isFinite(Number(totals.total)) ? Number(totals.total) : 0;
    const pending = totals && Number.isFinite(Number(totals.pending)) ? Number(totals.pending) : 0;
    const status = normalizeText(crawlStatus || "", "idle", 20);
    const canPublish = status === "success" && total > 0 && pending === 0;
    if (els.reviewPublishGuides) {
      els.reviewPublishGuides.disabled = !canPublish;
    }
    if (els.platformPublishGuides) {
      els.platformPublishGuides.disabled = !canPublish;
    }
    const setTitle = (message) => {
      if (els.reviewPublishGuides) {
        els.reviewPublishGuides.title = message;
      }
      if (els.platformPublishGuides) {
        els.platformPublishGuides.title = message;
      }
    };
    if (canPublish) {
      setTitle("Đã duyệt đủ. Có thể xuất bản vào chatbot.");
      return;
    }
    if (status !== "success") {
      setTitle("Hãy quét website thành công trước khi xuất bản.");
      return;
    }
    if (total <= 0) {
      setTitle("Chưa có bản nháp hướng dẫn để xuất bản.");
      return;
    }
    setTitle("Cần duyệt hết toàn bộ hướng dẫn trước khi xuất bản.");
  }

  function renderReviewSummary(payload) {
    if (!els.reviewSummary) {
      return;
    }
    if (!payload || typeof payload !== "object") {
      currentCrawlStatus = "idle";
      els.reviewSummary.textContent = "Chưa nạp bản nháp cần duyệt.";
      updateReviewPublishState({ total: 0, pending: 0 }, "idle");
      return;
    }
    const totals = payload.totals && typeof payload.totals === "object"
      ? payload.totals
      : computeReviewTotals(payload.guides);
    const crawlStatus = normalizeText(payload.crawlStatus || "idle", "idle", 30);
    currentCrawlStatus = crawlStatus;
    const lines = [];
    lines.push(`Website: ${payload.projectId || "-"}`);
    lines.push(`Trạng thái quét: ${crawlStatus}`);
    lines.push(`Tổng hướng dẫn cần duyệt: ${totals.total || 0}`);
    lines.push(`Đã duyệt: ${totals.reviewed || 0}`);
    lines.push(`Chưa duyệt: ${totals.pending || 0}`);
    if ((totals.pending || 0) > 0) {
      lines.push("Bạn cần duyệt hết toàn bộ trước khi xuất bản.");
    } else if ((totals.total || 0) > 0 && crawlStatus === "success") {
      lines.push("Đã duyệt đầy đủ, có thể xuất bản vào chatbot.");
    }
    els.reviewSummary.textContent = lines.join("\n");
    updateReviewPublishState(totals, crawlStatus);
  }

  function renderReviewDraftList(guides) {
    if (!els.reviewGuidesList) {
      return;
    }
    reviewDraftItems = Array.isArray(guides) ? guides.slice() : [];
    els.reviewGuidesList.innerHTML = "";
    if (reviewDraftItems.length === 0) {
      const empty = document.createElement("p");
      empty.className = "hint";
      empty.textContent = "Chưa có bản nháp cần duyệt. Hãy quét website rồi bấm “Nạp bản nháp cần duyệt”.";
      els.reviewGuidesList.appendChild(empty);
      return;
    }

    reviewDraftItems.forEach((guide, index) => {
      const item = document.createElement("article");
      item.className = "review-item";
      item.setAttribute("data-guide-id", guide.id || `guide_${index + 1}`);
      const sourceUrls = Array.isArray(guide.sourceUrls) ? guide.sourceUrls : [];
      const sourceHtml = sourceUrls
        .map((url) => {
          const safe = escapeHtml(url);
          return `<a href="${safe}" target="_blank" rel="noopener noreferrer">${safe}</a>`;
        })
        .join(" | ");
      const checkedAttr = guide.reviewed ? "checked" : "";

      item.innerHTML = `
        <div class="review-item-head">
          <h3>#${index + 1} ${escapeHtml(guide.title || "Hướng dẫn")}</h3>
          <label>
            <input type="checkbox" class="review-approved" ${checkedAttr} />
            Đã duyệt nội dung này
          </label>
        </div>
        <div class="review-grid">
          <label>Tiêu đề
            <input type="text" class="review-title" value="${escapeHtml(guide.title || "")}" />
          </label>
          <label>Từ khóa (phân tách dấu phẩy)
            <input type="text" class="review-keywords" value="${escapeHtml(
              Array.isArray(guide.intentKeywords) ? guide.intentKeywords.join(", ") : ""
            )}" />
          </label>
        </div>
        <label>Nút gợi ý nhanh (phân tách dấu phẩy)
          <input type="text" class="review-replies" value="${escapeHtml(
            Array.isArray(guide.quickReplies) ? guide.quickReplies.join(", ") : ""
          )}" />
        </label>
        <label>Nội dung trả lời
          <textarea class="review-answer" rows="7">${escapeHtml(guide.answer || "")}</textarea>
        </label>
        <p class="review-source">Nguồn tham khảo: ${sourceHtml || "-"}</p>
      `;
      els.reviewGuidesList.appendChild(item);
    });
  }

  function collectReviewDraftFromForm() {
    if (!els.reviewGuidesList) {
      return [];
    }
    const rows = Array.from(els.reviewGuidesList.querySelectorAll(".review-item"));
    const currentById = new Map();
    reviewDraftItems.forEach((item) => {
      if (item && item.id) {
        currentById.set(item.id, item);
      }
    });
    const guides = [];
    rows.forEach((row, index) => {
      const id = normalizeText(row.getAttribute("data-guide-id"), `guide_${index + 1}`, 120);
      const original = currentById.get(id) || {};
      const titleInput = row.querySelector(".review-title");
      const keywordsInput = row.querySelector(".review-keywords");
      const repliesInput = row.querySelector(".review-replies");
      const answerInput = row.querySelector(".review-answer");
      const approvedInput = row.querySelector(".review-approved");

      const title = normalizeText(titleInput ? titleInput.value : "", `Hướng dẫn ${index + 1}`, 180);
      const answer = normalizeText(answerInput ? answerInput.value : "", "", 2500);
      const intentKeywords = parseFlexibleList(keywordsInput ? keywordsInput.value : "").slice(0, 12);
      const quickReplies = parseFlexibleList(repliesInput ? repliesInput.value : "").slice(0, 3);
      const sourceUrls = Array.isArray(original.sourceUrls) ? original.sourceUrls.slice(0, 10) : [];

      guides.push({
        id,
        title,
        answer,
        intentKeywords,
        quickReplies,
        sourceUrls,
        reviewed: Boolean(approvedInput && approvedInput.checked)
      });
    });
    return guides;
  }

  function getPlatformProjectId() {
    const value = slugify(els.platformProjectId ? els.platformProjectId.value : "", "");
    if (value) {
      return value;
    }
    const baseUrl = normalizeUrlInput(els.platformBaseUrl ? els.platformBaseUrl.value : "");
    if (baseUrl) {
      return deriveProjectIdFromUrl(baseUrl);
    }
    return "default-site";
  }

  function collectPlatformProjectPayload() {
    const id = getPlatformProjectId();
    if (els.platformProjectId) {
      els.platformProjectId.value = id;
    }
    const requestHeaders = parseHeaderLines(els.platformAuthHeaders ? els.platformAuthHeaders.value : "");
    return {
      id,
      name: normalizeText(els.platformProjectName ? els.platformProjectName.value : "", id, 160),
      baseUrl: normalizeText(els.platformBaseUrl ? els.platformBaseUrl.value : "", "", 400),
      settings: {
        maxPages: clamp(els.platformMaxPages ? els.platformMaxPages.value : 40, 1, 200, 40),
        maxDepth: clamp(els.platformMaxDepth ? els.platformMaxDepth.value : 3, 0, 6, 3),
        timeoutMs: clamp(els.platformTimeoutMs ? els.platformTimeoutMs.value : 10000, 3000, 30000, 10000),
        includePaths: parseLines(els.platformIncludePaths ? els.platformIncludePaths.value : ""),
        excludePaths: parseLines(els.platformExcludePaths ? els.platformExcludePaths.value : ""),
        requestHeaders
      }
    };
  }

  function parseHeaderLines(text) {
    const lines = parseLines(text);
    if (lines.length === 0) {
      return {};
    }
    const out = {};
    for (let i = 0; i < lines.length; i += 1) {
      const line = lines[i];
      const index = line.indexOf(":");
      if (index <= 0) {
        continue;
      }
      const key = normalizeText(line.slice(0, index), "", 80).toLowerCase();
      const value = normalizeText(line.slice(index + 1), "", 12000);
      if (!key || !value) {
        continue;
      }
      if (!/^[a-z0-9-]+$/i.test(key)) {
        continue;
      }
      out[key] = value;
      if (Object.keys(out).length >= 20) {
        break;
      }
    }
    return out;
  }

  function formatHeaderLines(headers) {
    const source = headers && typeof headers === "object" ? headers : {};
    const keys = Object.keys(source);
    if (keys.length === 0) {
      return "";
    }
    return keys
      .map((key) => {
        const value = normalizeText(source[key], "", 12000);
        if (!value) {
          return "";
        }
        return `${key}: ${value}`;
      })
      .filter(Boolean)
      .join("\n");
  }

  function renderPlatformProject(project) {
    if (!project || typeof project !== "object") {
      currentCrawlStatus = "idle";
      renderPlatformSummary(null);
      reviewDraftItems = [];
      renderReviewDraftList([]);
      renderReviewSummary(null);
      if (els.platformAuthHeaders) {
        els.platformAuthHeaders.value = "";
      }
      if (els.platformPublishGuides) {
        els.platformPublishGuides.disabled = true;
        els.platformPublishGuides.title = "Hãy quét website để tạo hướng dẫn trước.";
      }
      return;
    }
    if (els.platformProjectId) {
      els.platformProjectId.value = project.id || "";
    }
    if (els.platformProjectName) {
      els.platformProjectName.value = project.name || "";
    }
    if (els.platformBaseUrl) {
      els.platformBaseUrl.value = project.baseUrl || "";
    }
    const settings = project.settings && typeof project.settings === "object" ? project.settings : {};
    if (els.platformMaxPages) {
      els.platformMaxPages.value = String(settings.maxPages ?? 40);
    }
    if (els.platformMaxDepth) {
      els.platformMaxDepth.value = String(settings.maxDepth ?? 3);
    }
    if (els.platformTimeoutMs) {
      els.platformTimeoutMs.value = String(settings.timeoutMs ?? 10000);
    }
    if (els.platformIncludePaths) {
      els.platformIncludePaths.value = Array.isArray(settings.includePaths) ? settings.includePaths.join("\n") : "";
    }
    if (els.platformExcludePaths) {
      els.platformExcludePaths.value = Array.isArray(settings.excludePaths) ? settings.excludePaths.join("\n") : "";
    }
    if (els.platformAuthHeaders) {
      els.platformAuthHeaders.value = formatHeaderLines(settings.requestHeaders);
    }
    const crawl = project.crawl && typeof project.crawl === "object" ? project.crawl : {};
    currentCrawlStatus = normalizeText(crawl.status || "idle", "idle", 30);
    const summary = crawl.summary && typeof crawl.summary === "object" ? crawl.summary : {};
    const guides = Array.isArray(crawl.guides) ? crawl.guides : [];
    renderReviewDraftList(guides);
    renderReviewSummary({
      projectId: project.id || "",
      guides,
      totals: computeReviewTotals(guides),
      crawlStatus: crawl.status || "idle"
    });
    renderPlatformSummary(project);
    syncQuickFormFromMain();
  }

  function collectKnowledgeBase() {
    const rows = [];
    const elements = els.kbList.querySelectorAll(".kb-row");
    elements.forEach((rowElement, index) => {
      const keywords = parseCsv(rowElement.querySelector(".kb-keywords").value);
      const answer = normalizeText(rowElement.querySelector(".kb-answer").value, "", 2000);
      const quickReplies = parseCsv(rowElement.querySelector(".kb-replies").value);
      if (!answer) {
        return;
      }
      rows.push({
        id: `kb_${index + 1}`,
        keywords,
        answer,
        quickReplies
      });
    });
    return rows;
  }

  function collectConfigFromForm() {
    return {
      brand: {
        name: normalizeText(els.brandName.value, DEFAULT_CONFIG.brand.name, 80),
        agentName: normalizeText(els.agentName.value, DEFAULT_CONFIG.brand.agentName, 80),
        launcherLabel: normalizeText(els.launcherLabel.value, DEFAULT_CONFIG.brand.launcherLabel, 60),
        welcomeTitle: normalizeText(els.welcomeTitle.value, DEFAULT_CONFIG.brand.welcomeTitle, 120),
        welcomeMessage: normalizeText(els.welcomeMessage.value, DEFAULT_CONFIG.brand.welcomeMessage, 500),
        inputPlaceholder: normalizeText(
          els.inputPlaceholder.value,
          DEFAULT_CONFIG.brand.inputPlaceholder,
          100
        ),
        logoDataUrl: currentLogoDataUrl || ""
      },
      theme: {
        primaryColor: els.themePrimary.value,
        launcherBg: els.themeLauncherBg.value,
        textOnPrimary: els.themeTextOnPrimary.value,
        panelBg: els.themePanelBg.value,
        botBubbleBg: els.themeBotBubble.value,
        userBubbleBg: els.themeUserBubble.value
      },
      layout: {
        position: els.layoutPosition.value === "left" ? "left" : "right",
        bottom: clamp(els.layoutBottom.value, 0, 120, DEFAULT_CONFIG.layout.bottom),
        side: clamp(els.layoutSide.value, 0, 120, DEFAULT_CONFIG.layout.side),
        zIndex: clamp(els.layoutZIndex.value, 1000, 2147483000, DEFAULT_CONFIG.layout.zIndex)
      },
      behavior: {
        showWelcomeOnOpen: els.behaviorShowWelcome.checked,
        startOpen: els.behaviorStartOpen.checked,
        typingDelayMs: clamp(els.behaviorTypingDelay.value, 0, 2000, DEFAULT_CONFIG.behavior.typingDelayMs),
        maxMessages: clamp(els.behaviorMaxMessages.value, 20, 500, DEFAULT_CONFIG.behavior.maxMessages)
      },
      quickStartButtons: parseLines(els.quickStartButtons.value),
      knowledgeBase: collectKnowledgeBase(),
      fallbackAnswer: normalizeText(els.fallbackAnswer.value, DEFAULT_CONFIG.fallbackAnswer, 2000)
    };
  }

  function ensureBundleState() {
    if (!staticBundle) {
      staticBundle = buildDefaultBundle();
    }
  }

  function persistBundleState() {
    ensureBundleState();
    persistTopLevelIntoActiveWorkspace();
    staticBundle.updatedAt = new Date().toISOString();
    writeBundleCache(staticBundle);
  }

  async function fetchBundleFromHost() {
    const response = await fetch(STATIC_BUNDLE_URL, {
      method: "GET",
      cache: "no-store"
    });
    if (!response.ok) {
      throw new Error(`Không thể tải ${STATIC_BUNDLE_URL} (HTTP ${response.status}).`);
    }
    const raw = await response.json();
    return normalizeBundle(raw);
  }

  async function loadBundleFromHost() {
    staticBundle = await fetchBundleFromHost();
    const preferredWorkspaceId = normalizeText(currentWorkspace && currentWorkspace.id ? currentWorkspace.id : "", "", 120);
    if (preferredWorkspaceId) {
      const hasPreferredWorkspace = Array.isArray(staticBundle.workspaces)
        && staticBundle.workspaces.some((item) => item && item.id === preferredWorkspaceId);
      if (hasPreferredWorkspace) {
        staticBundle.activeWorkspaceId = preferredWorkspaceId;
      }
    }
    writeBundleCache(staticBundle);
    return staticBundle;
  }

  async function loadConfig() {
    ensureBundleState();
    renderConfig(staticBundle.config);
  }

  async function saveConfig() {
    ensureBundleState();
    staticBundle.config = mergeConfig(collectConfigFromForm());
    persistBundleState();
    renderConfig(staticBundle.config);
    setStatus("Đã lưu tạm cấu hình trên trình duyệt.", "success");
    return {
      config: staticBundle.config
    };
  }

  async function loadDocuments() {
    ensureBundleState();
    renderDocuments(staticBundle.documents);
  }

  async function addDocuments(documents) {
    ensureBundleState();
    if (!Array.isArray(documents) || documents.length === 0) {
      throw new Error("Không có tài liệu hợp lệ để thêm.");
    }
    const next = Array.isArray(staticBundle.documents) ? staticBundle.documents.slice() : [];
    const now = new Date().toISOString();
    documents.forEach((item, index) => {
      const normalized = normalizeDocumentRecord(item, next.length + index);
      if (!normalized) {
        return;
      }
      normalized.createdAt = now;
      normalized.updatedAt = now;
      normalized.id = normalizeDocId(normalized.id, `doc_${Date.now()}_${Math.floor(Math.random() * 10000)}`);
      next.push(normalized);
    });
    staticBundle.documents = next;
    persistBundleState();
    renderDocuments(staticBundle.documents);
  }

  async function deleteDocument(id) {
    ensureBundleState();
    const docId = normalizeText(id, "", 160);
    staticBundle.documents = (Array.isArray(staticBundle.documents) ? staticBundle.documents : [])
      .filter((item) => normalizeText(item && item.id ? item.id : "", "", 160) !== docId);
    persistBundleState();
    renderDocuments(staticBundle.documents);
  }

  async function updateDocument(id, payload) {
    ensureBundleState();
    const docId = normalizeText(id, "", 160);
    const source = payload && typeof payload === "object" ? payload : {};
    const now = new Date().toISOString();
    const next = (Array.isArray(staticBundle.documents) ? staticBundle.documents : []).map((item) => {
      if (normalizeText(item && item.id ? item.id : "", "", 160) !== docId) {
        return item;
      }
      const title = normalizeText(source.title, normalizeText(item.title, "Không tiêu đề", 160), 160);
      const text = normalizeText(source.text, normalizeText(item.text, "", 120000), 120000);
      const tags = Array.isArray(source.tags) ? source.tags : parseCsv(source.tags || "");
      return {
        ...item,
        title,
        text,
        tags: tags.map((tag) => normalizeText(tag, "", 40)).filter(Boolean).slice(0, 20),
        updatedAt: now,
        length: text.length
      };
    });
    staticBundle.documents = next;
    persistBundleState();
    renderDocuments(staticBundle.documents);
  }

  function findPlatformProject(projectId) {
    ensureBundleState();
    const normalizedId = slugify(projectId, "");
    if (!normalizedId) {
      return null;
    }
    return (Array.isArray(staticBundle.platformProjects) ? staticBundle.platformProjects : [])
      .find((item) => normalizeText(item && item.id ? item.id : "", "", 120) === normalizedId) || null;
  }

  function upsertPlatformProject(project) {
    ensureBundleState();
    const payload = normalizePlatformProject(project, 0);
    const list = Array.isArray(staticBundle.platformProjects) ? staticBundle.platformProjects.slice() : [];
    const index = list.findIndex((item) => item && item.id === payload.id);
    if (index >= 0) {
      list[index] = {
        ...list[index],
        ...payload
      };
    } else {
      list.push(payload);
    }
    staticBundle.platformProjects = list;
    persistBundleState();
    return payload;
  }

  function foldText(text) {
    return (text || "")
      .toString()
      .normalize("NFD")
      .replace(/[\u0300-\u036f]/g, "")
      .replace(/[đĐ]/g, "d")
      .toLowerCase()
      .replace(/\s+/g, " ")
      .trim();
  }

  function cleanActionText(value) {
    const text = normalizeText(value, "", 80)
      .replace(/^[\s\-–—•|:;,.]+/g, "")
      .replace(/[\s\-–—|:;,.]+$/g, "")
      .trim();
    if (!text || text.length < 2) {
      return "";
    }
    if (!/[0-9A-Za-zÀ-ỹ]/.test(text)) {
      return "";
    }
    return text;
  }

  function classifyPageType(url, title, heading) {
    const source = foldText(`${url} ${title} ${heading}`);
    if (/dang nhap|login|sign in/.test(source)) {
      return "dang_nhap";
    }
    if (/dang ky|register|tao tai khoan|signup|sign up/.test(source)) {
      return "dang_ky";
    }
    if (/gio hang|cart|basket/.test(source)) {
      return "gio_hang";
    }
    if (/thanh toan|checkout|payment/.test(source)) {
      return "thanh_toan";
    }
    if (/lien he|ho tro|support|contact/.test(source)) {
      return "lien_he";
    }
    if (/tim kiem|search/.test(source)) {
      return "tim_kiem";
    }
    if (/tai khoan|ho so|profile/.test(source)) {
      return "tai_khoan";
    }
    if (/huong dan|faq|tro giup/.test(source)) {
      return "tro_giup";
    }
    if (/trang chu|home/.test(source)) {
      return "trang_chu";
    }
    return "noi_dung";
  }

  function pageTypeKeywords(pageType) {
    const map = {
      dang_nhap: ["đăng nhập", "login", "vào tài khoản"],
      dang_ky: ["đăng ký", "tạo tài khoản", "register"],
      gio_hang: ["giỏ hàng", "thêm vào giỏ", "cart"],
      thanh_toan: ["thanh toán", "checkout", "payment"],
      lien_he: ["liên hệ", "hỗ trợ", "tư vấn"],
      tim_kiem: ["tìm kiếm", "search", "tra cứu"],
      tai_khoan: ["tài khoản", "hồ sơ", "profile"],
      tro_giup: ["hướng dẫn", "trợ giúp", "faq"],
      trang_chu: ["bắt đầu", "trang chủ", "điều hướng"],
      noi_dung: ["sử dụng", "thao tác", "hướng dẫn"]
    };
    return map[pageType] ? map[pageType].slice() : map.noi_dung.slice();
  }

  function buildKeywordsFromTitle(title) {
    const folded = foldText(title);
    if (!folded) {
      return [];
    }
    return folded
      .split(/[^a-z0-9]+/g)
      .map((item) => item.trim())
      .filter((item) => item.length >= 3)
      .slice(0, 4);
  }

  function extractQuickHint(doc) {
    const paragraph = normalizeText(
      (doc.querySelector("main p, article p, section p, p") || {}).textContent || "",
      "",
      220
    );
    return paragraph || "Thực hiện tuần tự theo nội dung hiển thị trên trang.";
  }

  function extractActionLabels(doc) {
    const labels = [];
    const seen = new Set();
    const selectors = [
      "button",
      "a",
      "input[type='submit']",
      "input[type='button']"
    ];
    selectors.forEach((selector) => {
      const nodes = Array.from(doc.querySelectorAll(selector));
      nodes.forEach((node) => {
        const raw = selector.startsWith("input")
          ? node.getAttribute("value") || ""
          : node.textContent || "";
        const text = cleanActionText(raw);
        const key = foldText(text);
        if (!key || seen.has(key)) {
          return;
        }
        if (labels.length >= 10) {
          return;
        }
        seen.add(key);
        labels.push(text);
      });
    });
    return labels;
  }

  function extractLinks(doc, currentUrl, baseOrigin) {
    const links = [];
    const seen = new Set();
    const anchors = Array.from(doc.querySelectorAll("a[href]"));
    anchors.forEach((anchor) => {
      const href = normalizeText(anchor.getAttribute("href"), "", 800);
      if (!href || href.startsWith("#") || href.startsWith("javascript:") || href.startsWith("mailto:") || href.startsWith("tel:")) {
        return;
      }
      try {
        const parsed = new URL(href, currentUrl);
        if (parsed.origin !== baseOrigin) {
          return;
        }
        parsed.hash = "";
        const normalized = parsed.href;
        if (seen.has(normalized)) {
          return;
        }
        seen.add(normalized);
        links.push(normalized);
      } catch (_) {}
    });
    return links;
  }

  function isUrlAllowed(url, includePaths, excludePaths) {
    const includes = Array.isArray(includePaths) ? includePaths : [];
    const excludes = Array.isArray(excludePaths) ? excludePaths : [];
    const foldedUrl = foldText(url);
    if (includes.length > 0) {
      const matchInclude = includes.some((item) => foldedUrl.includes(foldText(item)));
      if (!matchInclude) {
        return false;
      }
    }
    const matchExclude = excludes.some((item) => foldedUrl.includes(foldText(item)));
    return !matchExclude;
  }

  async function fetchHtmlPage(url, timeoutMs) {
    const controller = new AbortController();
    const timer = window.setTimeout(() => controller.abort(), timeoutMs);
    try {
      const response = await fetch(url, {
        method: "GET",
        cache: "no-store",
        credentials: "include",
        signal: controller.signal
      });
      if (!response.ok) {
        throw new Error(`HTTP ${response.status}`);
      }
      const contentType = normalizeText(response.headers.get("content-type") || "", "", 200).toLowerCase();
      if (contentType && !contentType.includes("text/html")) {
        throw new Error("Trang không phải HTML.");
      }
      return await response.text();
    } finally {
      window.clearTimeout(timer);
    }
  }

  function buildGuideFromPage(page, index, projectName) {
    const pageType = classifyPageType(page.url, page.title, page.heading);
    const keywords = pageTypeKeywords(pageType);
    const titleText = normalizeText(page.heading || page.title, `Hướng dẫn ${index + 1}`, 180);
    const actionPrimary = page.actions[0] ? `Bấm "${page.actions[0]}".` : "Thực hiện theo nội dung hiển thị trên trang.";
    const actionSecondary = page.actions[1] ? `- **Nếu cần:** Tiếp tục với "${page.actions[1]}".` : "";
    const hint = normalizeText(page.quickHint, "", 260);
    const answerLines = [
      "Mình hướng dẫn bạn theo trang này:",
      `- **Mở trang:** ${page.url}`,
      `- **Xác nhận:** Bạn đang ở trang "${normalizeText(page.title, titleText, 180)}".`,
      `- **Thao tác chính:** ${actionPrimary}`,
      actionSecondary,
      "- **Nếu gặp lỗi:** Quay lại bước trước hoặc liên hệ hỗ trợ."
    ].filter(Boolean);
    if (hint) {
      answerLines.push("", `**Gợi ý nhanh:** ${hint}`);
    }
    answerLines.push("", `**Tham khảo thêm:** ${page.url}`);
    const extraFromTitle = buildKeywordsFromTitle(titleText);
    return {
      id: normalizeDocId(`guide_${pageType}_${index + 1}`, `guide_${index + 1}`),
      title: titleText,
      answer: answerLines.join("\n"),
      intentKeywords: [...keywords, ...extraFromTitle].slice(0, 12),
      quickReplies: keywords.slice(0, 3),
      sourceUrls: [page.url],
      reviewed: false
    };
  }

  async function crawlWebsiteInBrowser(project, crawlOverride) {
    const payload = crawlOverride && typeof crawlOverride === "object" ? crawlOverride : {};
    const options = {
      maxPages: clamp(payload.maxPages ?? project.settings.maxPages, 1, 200, 40),
      maxDepth: clamp(payload.maxDepth ?? project.settings.maxDepth, 0, 6, 3),
      timeoutMs: clamp(payload.timeoutMs ?? project.settings.timeoutMs, 3000, 30000, 10000),
      includePaths: Array.isArray(payload.includePaths) ? payload.includePaths : project.settings.includePaths,
      excludePaths: Array.isArray(payload.excludePaths) ? payload.excludePaths : project.settings.excludePaths
    };
    const startUrl = normalizeUrlInput(project.baseUrl);
    if (!startUrl) {
      throw new Error("Website chưa có URL hợp lệ để quét.");
    }
    const baseOrigin = new URL(startUrl).origin;
    const currentOrigin = window.location.origin;
    if (baseOrigin !== currentOrigin) {
      throw new Error(
        `Bản không Node chỉ quét được website cùng domain với trang admin (${currentOrigin}). `
        + `Website bạn nhập là ${baseOrigin} nên bị chặn CORS. `
        + "Bạn có thể: 1) nhập dữ liệu thủ công/tải file, hoặc 2) dùng bản có backend để quét đa domain."
      );
    }
    const queue = [{ url: startUrl, depth: 0 }];
    const visited = new Set();
    const pages = [];
    const errors = [];
    const pageTypeStats = {};
    const startedAt = new Date().toISOString();

    while (queue.length > 0 && pages.length < options.maxPages) {
      const node = queue.shift();
      if (!node || visited.has(node.url)) {
        continue;
      }
      visited.add(node.url);
      if (!isUrlAllowed(node.url, options.includePaths, options.excludePaths)) {
        continue;
      }
      try {
        const html = await fetchHtmlPage(node.url, options.timeoutMs);
        const parser = new DOMParser();
        const doc = parser.parseFromString(html, "text/html");
        Array.from(doc.querySelectorAll("script,style,noscript,template")).forEach((item) => item.remove());
        const title = normalizeText(doc.title || "", "", 180);
        const heading = normalizeText((doc.querySelector("h1,h2,h3") || {}).textContent || "", "", 180);
        const actions = extractActionLabels(doc);
        const links = extractLinks(doc, node.url, baseOrigin);
        const quickHint = extractQuickHint(doc);
        const pageType = classifyPageType(node.url, title, heading);
        pageTypeStats[pageType] = (pageTypeStats[pageType] || 0) + 1;
        pages.push({
          url: node.url,
          depth: node.depth,
          title: title || heading || node.url,
          heading,
          actions,
          quickHint,
          pageType
        });
        if (node.depth < options.maxDepth) {
          links.forEach((link) => {
            if (visited.has(link)) {
              return;
            }
            if (!isUrlAllowed(link, options.includePaths, options.excludePaths)) {
              return;
            }
            queue.push({ url: link, depth: node.depth + 1 });
          });
        }
      } catch (error) {
        errors.push({
          url: node.url,
          message: normalizeText(error && error.message ? error.message : "Không đọc được trang", "Không đọc được trang", 260)
        });
      }
    }

    const guides = pages.map((page, index) => buildGuideFromPage(page, index, project.name));
    const review = computeReviewTotals(guides);
    const loginPageCount = Number(pageTypeStats.dang_nhap || 0);
    return {
      startedAt,
      endedAt: new Date().toISOString(),
      pages,
      guides,
      errors,
      summary: {
        crawledPages: pages.length,
        generatedGuides: guides.length,
        errors: errors.length,
        pageTypeStats,
        loginPageRatio: pages.length > 0 ? loginPageCount / pages.length : 0,
        review
      },
      options
    };
  }

  function buildProjectGuideDocuments(project) {
    const guides = normalizeGuideDrafts(project && project.crawl ? project.crawl.guides : [], project.id);
    const now = new Date().toISOString();
    return guides.map((guide, index) => {
      const keywords = Array.isArray(guide.intentKeywords) ? guide.intentKeywords : [];
      const sourceUrls = Array.isArray(guide.sourceUrls) ? guide.sourceUrls : [];
      const lines = [];
      lines.push(`**Mục tiêu:** ${guide.title}`);
      lines.push("");
      lines.push(guide.answer);
      if (sourceUrls.length > 0) {
        lines.push("");
        lines.push(`**Trang nguồn:** ${sourceUrls.join(" | ")}`);
      }
      return {
        id: normalizeDocId(`guide_${project.id}_${index + 1}`, `guide_${index + 1}`),
        title: `[${project.name}] ${guide.title}`,
        text: lines.join("\n"),
        tags: [`project:${project.id}`, "auto-guide", "hướng-dẫn", ...keywords.map((item) => foldText(item)).filter(Boolean)].slice(0, 20),
        createdAt: now,
        updatedAt: now,
        length: lines.join("\n").length,
        meta: {
          projectId: project.id,
          type: "guide",
          sourceUrls
        }
      };
    });
  }

  function buildProjectGuideKnowledgeBase(project) {
    const guides = normalizeGuideDrafts(project && project.crawl ? project.crawl.guides : [], project.id);
    const rows = [];
    guides.forEach((guide, index) => {
      const keywords = Array.isArray(guide.intentKeywords) ? guide.intentKeywords.slice(0, 10) : [];
      if (keywords.length === 0) {
        return;
      }
      const sourceUrls = Array.isArray(guide.sourceUrls) ? guide.sourceUrls.slice(0, 2) : [];
      const answer = sourceUrls.length > 0
        ? `${guide.answer}\n\n**Tham khảo thêm:** ${sourceUrls.join(" | ")}`
        : guide.answer;
      rows.push({
        id: `auto_${project.id}_${index + 1}`,
        keywords,
        answer,
        quickReplies: Array.isArray(guide.quickReplies) && guide.quickReplies.length > 0
          ? guide.quickReplies.slice(0, 3)
          : keywords.slice(0, 3)
      });
    });
    return rows.slice(0, 40);
  }

  async function loadPlatformProject() {
    ensureBundleState();
    const projectId = getPlatformProjectId();
    const project = findPlatformProject(projectId);
    if (!project) {
      renderPlatformSummary({
        id: projectId,
        name: "(chưa tạo)",
        baseUrl: "",
        settings: { requestHeaders: {} },
        crawl: { status: "idle", summary: {} }
      });
      renderReviewDraftList([]);
      renderReviewSummary(null);
      return null;
    }
    renderPlatformProject(project);
    return project;
  }

  async function savePlatformProject() {
    ensureBundleState();
    const payload = collectPlatformProjectPayload();
    if (!payload.baseUrl) {
      throw new Error("Bạn cần nhập URL gốc của website.");
    }
    const existing = findPlatformProject(payload.id);
    const merged = normalizePlatformProject({
      ...(existing || {}),
      ...payload,
      crawl: existing && existing.crawl ? existing.crawl : {
        status: "idle",
        startedAt: "",
        endedAt: "",
        summary: {},
        guides: [],
        pages: [],
        errors: []
      }
    }, 0);
    const saved = upsertPlatformProject(merged);
    renderPlatformProject(saved);
    return saved;
  }

  async function runPlatformCrawl() {
    const project = await savePlatformProject();
    const crawlPayload = collectPlatformProjectPayload();
    const startedAt = new Date().toISOString();
    const running = {
      ...project,
      crawl: {
        ...(project.crawl || {}),
        status: "running",
        startedAt,
        endedAt: "",
        summary: {},
        errors: []
      }
    };
    upsertPlatformProject(running);
    renderPlatformProject(running);

    let result;
    try {
      result = await crawlWebsiteInBrowser(project, {
        maxPages: crawlPayload.settings.maxPages,
        maxDepth: crawlPayload.settings.maxDepth,
        timeoutMs: crawlPayload.settings.timeoutMs,
        includePaths: crawlPayload.settings.includePaths,
        excludePaths: crawlPayload.settings.excludePaths
      });
    } catch (error) {
      const message = normalizeText(
        error && error.message ? error.message : "Không thể quét website.",
        "Không thể quét website.",
        500
      );
      const failedProject = {
        ...project,
        settings: {
          ...project.settings,
          maxPages: crawlPayload.settings.maxPages,
          maxDepth: crawlPayload.settings.maxDepth,
          timeoutMs: crawlPayload.settings.timeoutMs,
          includePaths: crawlPayload.settings.includePaths,
          excludePaths: crawlPayload.settings.excludePaths
        },
        crawl: {
          status: "failed",
          startedAt,
          endedAt: new Date().toISOString(),
          summary: {
            crawledPages: 0,
            generatedGuides: 0,
            errors: 1,
            authHint: message
          },
          guides: [],
          pages: [],
          errors: [
            {
              url: normalizeText(project.baseUrl, "-", 400),
              message
            }
          ]
        }
      };
      upsertPlatformProject(failedProject);
      renderPlatformProject(failedProject);
      throw new Error(message);
    }
    const doneProject = {
      ...project,
      settings: {
        ...project.settings,
        maxPages: result.options.maxPages,
        maxDepth: result.options.maxDepth,
        timeoutMs: result.options.timeoutMs,
        includePaths: result.options.includePaths,
        excludePaths: result.options.excludePaths
      },
      crawl: {
        status: "success",
        startedAt: result.startedAt,
        endedAt: result.endedAt,
        summary: result.summary,
        guides: normalizeGuideDrafts(result.guides, project.id).map((guide) => ({
          ...guide,
          reviewed: false
        })),
        pages: result.pages,
        errors: result.errors
      }
    };
    upsertPlatformProject(doneProject);
    renderPlatformProject(doneProject);
    return {
      ok: true,
      project: doneProject,
      crawl: result
    };
  }

  async function publishPlatformGuides() {
    ensureBundleState();
    const projectId = getPlatformProjectId();
    const project = findPlatformProject(projectId);
    if (!project) {
      throw new Error("Không tìm thấy website để xuất bản.");
    }
    const guides = normalizeGuideDrafts(project && project.crawl ? project.crawl.guides : [], project.id);
    const totals = computeReviewTotals(guides);
    if (totals.total > 0 && totals.pending > 0) {
      throw new Error(`Bạn còn ${totals.pending}/${totals.total} hướng dẫn chưa duyệt.`);
    }
    if (project.crawl.status !== "success") {
      throw new Error("Website chưa quét xong. Hãy quét website trước khi đưa dữ liệu vào chatbot.");
    }
    if (guides.length <= 0) {
      throw new Error("Chưa có hướng dẫn nào được sinh ra. Hãy quét lại website.");
    }

    const docs = buildProjectGuideDocuments(project);
    const kb = buildProjectGuideKnowledgeBase(project);
    const projectTag = `project:${project.id}`;
    const baseDocs = (Array.isArray(staticBundle.documents) ? staticBundle.documents : [])
      .filter((item) => !Array.isArray(item.tags) || !item.tags.includes(projectTag));
    staticBundle.documents = [...baseDocs, ...docs];

    const config = mergeConfig(staticBundle.config);
    const filteredKb = Array.isArray(config.knowledgeBase)
      ? config.knowledgeBase.filter((row) => !normalizeText(row && row.id ? row.id : "", "", 200).startsWith(`auto_${project.id}_`))
      : [];
    staticBundle.config = {
      ...config,
      knowledgeBase: [...filteredKb, ...kb]
    };
    persistBundleState();
    renderConfig(staticBundle.config);
    renderDocuments(staticBundle.documents);
    renderPlatformProject(project);
    return {
      ok: true,
      published: {
        documents: docs.length,
        knowledgeBase: kb.length
      },
      review: totals
    };
  }

  async function loadReviewDraft() {
    const projectId = getPlatformProjectId();
    const project = findPlatformProject(projectId);
    if (!project) {
      throw new Error("Chưa có website để nạp bản nháp.");
    }
    const guides = normalizeGuideDrafts(project && project.crawl ? project.crawl.guides : [], project.id);
    const payload = {
      projectId,
      guides,
      totals: computeReviewTotals(guides),
      crawlStatus: normalizeText(project.crawl && project.crawl.status ? project.crawl.status : "idle", "idle", 20)
    };
    renderReviewDraftList(payload.guides);
    renderReviewSummary(payload);
    return payload;
  }

  async function saveReviewDraft(guidesOverride) {
    const projectId = getPlatformProjectId();
    const project = findPlatformProject(projectId);
    if (!project) {
      throw new Error("Không tìm thấy website để lưu bản nháp.");
    }
    const guides = normalizeGuideDrafts(Array.isArray(guidesOverride) ? guidesOverride : collectReviewDraftFromForm(), project.id);
    const totals = computeReviewTotals(guides);
    const nextProject = {
      ...project,
      crawl: {
        ...(project.crawl || {}),
        guides,
        summary: {
          ...(project.crawl && project.crawl.summary && typeof project.crawl.summary === "object" ? project.crawl.summary : {}),
          review: totals
        }
      }
    };
    upsertPlatformProject(nextProject);
    renderReviewDraftList(guides);
    renderReviewSummary({
      projectId,
      guides,
      totals,
      crawlStatus: normalizeText(nextProject.crawl && nextProject.crawl.status ? nextProject.crawl.status : "idle", "idle", 20)
    });
    return {
      ok: true,
      projectId,
      guides,
      totals,
      crawlStatus: normalizeText(nextProject.crawl && nextProject.crawl.status ? nextProject.crawl.status : "idle", "idle", 20)
    };
  }

  async function publishReviewedGuides() {
    const saved = await saveReviewDraft();
    if ((saved.totals && saved.totals.pending) > 0) {
      throw new Error(`Bạn còn ${saved.totals.pending} hướng dẫn chưa duyệt. Hãy duyệt đủ trước khi xuất bản.`);
    }
    const published = await publishPlatformGuides();
    await loadReviewDraft();
    return published;
  }

  async function createSessionFromCredentials() {
    throw new Error(
      "Bản chạy không cần Node không hỗ trợ đăng nhập tự động bằng tài khoản. Hãy đăng nhập sẵn website ở tab khác rồi bấm quét."
    );
  }

  function getEmbedSnippet(projectId) {
    const widgetUrl = new URL("../widget/embed.js", window.location.href).href;
    const configUrl = new URL(STATIC_BUNDLE_URL, window.location.href).href;
    const normalizedProjectId = slugify(projectId || "", "default-site");
    const projectAttr = normalizedProjectId ? ` data-project-id="${normalizedProjectId}"` : "";
    const configAttr = ` data-config-url="${configUrl}"`;
    const widgetKey = normalizeText(currentWorkspace && currentWorkspace.widgetKey ? currentWorkspace.widgetKey : "", "", 120);
    const widgetKeyAttr = widgetKey ? ` data-widget-key="${widgetKey}"` : "";
    return `<script src="${widgetUrl}"${configAttr}${widgetKeyAttr}${projectAttr} defer><\/script>`;
  }

  function renderEmbedCode(projectIdOverride) {
    const projectId = slugify(projectIdOverride || getPlatformProjectId(), "default-site");
    const code = getEmbedSnippet(projectId);
    if (els.embedCode) {
      els.embedCode.value = code;
    }
    if (els.quickEmbedCode) {
      els.quickEmbedCode.value = code;
    }
  }

  function renderQuickEmbedPreview() {
    const quickUrl = normalizeUrlInput(els.quickSiteUrl ? els.quickSiteUrl.value : "");
    if (quickUrl) {
      renderEmbedCode(deriveProjectIdFromUrl(quickUrl));
      return;
    }
    renderEmbedCode();
  }

  function buildDemoUrl(projectId) {
    const demoUrl = new URL("../demo/", window.location.href);
    const normalizedProjectId = slugify(projectId || getPlatformProjectId(), "default-site");
    const widgetKey = normalizeText(currentWorkspace && currentWorkspace.widgetKey ? currentWorkspace.widgetKey : "", "", 120);
    if (widgetKey) {
      demoUrl.searchParams.set("widgetKey", widgetKey);
    }
    if (normalizedProjectId) {
      demoUrl.searchParams.set("projectId", normalizedProjectId);
    }
    demoUrl.searchParams.set("previewLocal", "1");
    return demoUrl.href;
  }

  function syncQuickFormFromMain() {
    if (els.quickBrandName && els.brandName) {
      els.quickBrandName.value = normalizeText(els.brandName.value, DEFAULT_CONFIG.brand.name, 80);
    }
    if (els.quickAgentName && els.agentName) {
      els.quickAgentName.value = normalizeText(els.agentName.value, DEFAULT_CONFIG.brand.agentName, 80);
    }
    if (els.quickWelcomeMessage && els.welcomeMessage) {
      els.quickWelcomeMessage.value = normalizeText(
        els.welcomeMessage.value,
        DEFAULT_CONFIG.brand.welcomeMessage,
        500
      );
    }
    if (els.quickFallbackAnswer && els.fallbackAnswer) {
      els.quickFallbackAnswer.value = normalizeText(els.fallbackAnswer.value, DEFAULT_CONFIG.fallbackAnswer, 2000);
    }
    if (els.quickSiteUrl && els.platformBaseUrl) {
      const normalizedBase = normalizeUrlInput(els.platformBaseUrl.value);
      els.quickSiteUrl.value = normalizedBase || normalizeText(els.platformBaseUrl.value, "", 400);
    }
    if (els.quickLoginUrl && els.quickSiteUrl) {
      const defaultLoginUrl = buildDefaultLoginUrl(els.quickSiteUrl.value);
      if (!normalizeText(els.quickLoginUrl.value, "", 400)) {
        els.quickLoginUrl.value = defaultLoginUrl;
      }
    }
    renderQuickEmbedPreview();
  }

  function applyQuickFormToMain() {
    const brandName = normalizeText(
      els.quickBrandName ? els.quickBrandName.value : "",
      DEFAULT_CONFIG.brand.name,
      80
    );
    const normalizedUrl = normalizeUrlInput(els.quickSiteUrl ? els.quickSiteUrl.value : "");
    const projectId = deriveProjectIdFromUrl(normalizedUrl);
    const agentName = normalizeText(
      els.quickAgentName ? els.quickAgentName.value : "",
      `Trợ lý ${brandName.split(/\s+/g)[0] || "Aha"}`,
      80
    );
    const welcomeMessage = normalizeText(
      els.quickWelcomeMessage ? els.quickWelcomeMessage.value : "",
      DEFAULT_CONFIG.brand.welcomeMessage,
      500
    );
    const fallbackAnswer = normalizeText(
      els.quickFallbackAnswer ? els.quickFallbackAnswer.value : "",
      DEFAULT_CONFIG.fallbackAnswer,
      2000
    );
    const loginUrlFromForm = normalizeUrlInput(els.quickLoginUrl ? els.quickLoginUrl.value : "");
    const loginUrl = loginUrlFromForm || buildDefaultLoginUrl(normalizedUrl);
    const loginUsername = normalizeText(
      els.quickLoginUsername ? els.quickLoginUsername.value : "",
      "",
      200
    );
    const loginPassword = normalizeText(
      els.quickLoginPassword ? els.quickLoginPassword.value : "",
      "",
      200
    );
    const loginOtp = normalizeText(els.quickLoginOtp ? els.quickLoginOtp.value : "", "", 120);

    if (els.brandName) {
      els.brandName.value = brandName;
    }
    if (els.agentName) {
      els.agentName.value = agentName;
    }
    if (els.launcherLabel) {
      els.launcherLabel.value = brandName;
    }
    if (els.welcomeTitle) {
      els.welcomeTitle.value = "Xin chào, mình có thể hỗ trợ gì?";
    }
    if (els.welcomeMessage) {
      els.welcomeMessage.value = welcomeMessage;
    }
    if (els.inputPlaceholder) {
      els.inputPlaceholder.value = DEFAULT_CONFIG.brand.inputPlaceholder;
    }
    if (els.quickStartButtons) {
      els.quickStartButtons.value = DEFAULT_CONFIG.quickStartButtons.join("\n");
    }
    if (els.fallbackAnswer) {
      els.fallbackAnswer.value = fallbackAnswer;
    }
    if (els.platformProjectId) {
      els.platformProjectId.value = projectId;
    }
    if (els.platformProjectName) {
      els.platformProjectName.value = brandName;
    }
    if (els.platformBaseUrl) {
      els.platformBaseUrl.value = normalizedUrl;
    }
    if (els.platformMaxPages) {
      els.platformMaxPages.value = "60";
    }
    if (els.platformMaxDepth) {
      els.platformMaxDepth.value = "3";
    }
    if (els.platformTimeoutMs) {
      els.platformTimeoutMs.value = "10000";
    }
    renderEmbedCode(projectId);
    return {
      brandName,
      projectId,
      baseUrl: normalizedUrl,
      login: {
        loginUrl,
        username: loginUsername,
        password: loginPassword,
        otpCode: loginOtp
      }
    };
  }

  async function runQuickAutoDeploy() {
    const mapped = applyQuickFormToMain();
    if (!mapped.baseUrl) {
      throw new Error("Bạn cần nhập link website hợp lệ trước khi triển khai.");
    }

    const progress = [];
    progress.push(`Website: ${mapped.baseUrl}`);
    progress.push(`Mã website: ${mapped.projectId}`);
    setQuickStatus(`${progress.join("\n")}\nĐang lưu cấu hình cơ bản...`, "muted");

    const originalButtonText = els.quickAutoDeploy ? els.quickAutoDeploy.textContent : "";
    if (els.quickAutoDeploy) {
      els.quickAutoDeploy.disabled = true;
      els.quickAutoDeploy.textContent = "Đang triển khai...";
    }

    try {
      await saveConfig();
      progress.push("- Đã lưu giao diện và nội dung cơ bản.");
      await savePlatformProject().catch(() => null);
      progress.push("- Đã lưu cấu hình website và mã nhúng.");
      progress.push("- Chế độ thiết lập nhanh không chạy quét tự động.");
      progress.push("- Vào tab “Nội dung” để cập nhật Bộ câu trả lời mẫu và Kho tài liệu.");
      setQuickStatus(progress.join("\n"), "success");
      setStatus("Đã lưu thiết lập nhanh. Bạn có thể chuyển sang tab Nội dung để tối ưu trả lời.", "success");
    } finally {
      if (els.quickAutoDeploy) {
        els.quickAutoDeploy.disabled = false;
        els.quickAutoDeploy.textContent = originalButtonText || "Lưu thiết lập nhanh";
      }
    }
  }

  async function readFileAsDataUrl(file) {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.onload = () => resolve(reader.result);
      reader.onerror = () => reject(new Error("Không thể đọc ảnh đã chọn."));
      reader.readAsDataURL(file);
    });
  }

  async function readFileAsText(file) {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.onload = () => resolve(typeof reader.result === "string" ? reader.result : "");
      reader.onerror = () => reject(new Error(`Không thể đọc file: ${file.name}`));
      reader.readAsText(file);
    });
  }

  async function handleLogoFile(file) {
    if (!file) {
      return;
    }
    if (!/^image\//i.test(file.type)) {
      setStatus("Vui lòng chọn đúng file ảnh.", "error");
      return;
    }
    if (file.size > 2 * 1024 * 1024) {
      setStatus("Ảnh quá lớn. Tối đa 2MB.", "error");
      return;
    }
    try {
      const dataUrl = await readFileAsDataUrl(file);
      if (typeof dataUrl !== "string") {
        throw new Error("Dữ liệu ảnh không hợp lệ.");
      }
      currentLogoDataUrl = dataUrl;
      renderLogoPreview(currentLogoDataUrl);
      setStatus("Đã tải logo. Bấm Lưu cấu hình để áp dụng.", "success");
    } catch (error) {
      setStatus(error.message || "Không thể tải ảnh.", "error");
    }
  }

  async function handleManualDocumentAdd() {
    const title = normalizeText(els.docTitle.value, "", 160);
    const text = normalizeText(els.docText.value, "", 120000);
    const tags = parseCsv(els.docTags.value);
    if (!title || !text) {
      setStatus("Cần nhập tiêu đề và nội dung tài liệu.", "error");
      return;
    }
    try {
      setStatus("Đang thêm tài liệu...", "muted");
      await addDocuments([{ title, text, tags }]);
      els.docTitle.value = "";
      els.docTags.value = "";
      els.docText.value = "";
      setStatus("Đã thêm tài liệu.", "success");
    } catch (error) {
      setStatus(error.message || "Thêm tài liệu thất bại.", "error");
    }
  }

  async function handleFileUpload() {
    const files = els.docUpload.files ? Array.from(els.docUpload.files) : [];
    if (files.length === 0) {
      setStatus("Vui lòng chọn ít nhất một file.", "error");
      return;
    }
    try {
      setStatus("Đang đọc file...", "muted");
      const docs = [];
      for (let i = 0; i < files.length; i += 1) {
        const file = files[i];
        if (file.size > 2 * 1024 * 1024) {
          continue;
        }
        const fileName = normalizeText(file.name, `file_${i + 1}`, 160);
        const title = fileName.replace(/\.[^/.]+$/, "");
        const ext = fileName.includes(".") ? fileName.split(".").pop().toLowerCase() : "";
        if (ext === "xlsx") {
          const parsedRows = parseDocumentsFromXlsxRows(await parseXlsxFile(file));
          if (!Array.isArray(parsedRows) || parsedRows.length === 0) {
            throw new Error(`File XLSX ${fileName} chưa có dòng dữ liệu hợp lệ.`);
          }
          docs.push(...parsedRows);
          continue;
        }
        const text = await readFileAsText(file);
        if (ext === "csv") {
          const parsedRows = parseDocumentsFromCsvText(text);
          if (!Array.isArray(parsedRows) || parsedRows.length === 0) {
            throw new Error(`File CSV ${fileName} chưa có dòng dữ liệu hợp lệ.`);
          }
          docs.push(...parsedRows);
          continue;
        }
        if (ext === "json") {
          let parsedRows = [];
          try {
            parsedRows = parseDocumentsFromJsonText(text);
          } catch (_) {
            parsedRows = [];
          }
          if (parsedRows.length > 0) {
            docs.push(...parsedRows);
            continue;
          }
        }
        const normalizedText = normalizeText(text, "", 120000);
        if (!normalizedText) {
          continue;
        }
        const tags = ext ? [ext] : [];
        docs.push({
          title: title || fileName,
          text: normalizedText,
          tags
        });
      }
      if (docs.length === 0) {
        setStatus("Không có nội dung hợp lệ để nhập.", "error");
        return;
      }
      setStatus(`Đang tải lên ${docs.length} tài liệu...`, "muted");
      await addDocuments(docs);
      els.docUpload.value = "";
      setStatus(`Đã nhập ${docs.length} tài liệu.`, "success");
    } catch (error) {
      setStatus(error.message || "Tải file thất bại.", "error");
    }
  }

  async function importKnowledgeBaseFromFile() {
    const file = els.kbUpload.files && els.kbUpload.files[0] ? els.kbUpload.files[0] : null;
    if (!file) {
      setStatus("Vui lòng chọn file mẫu đã điền dữ liệu.", "error");
      return;
    }
    try {
      setStatus("Đang đọc file bộ câu trả lời...", "muted");
      const lowerName = (file.name || "").toLowerCase();
      let rows = [];
      if (lowerName.endsWith(".json")) {
        const text = await readFileAsText(file);
        rows = parseKbFromJsonText(text);
      } else if (lowerName.endsWith(".csv")) {
        const text = await readFileAsText(file);
        rows = parseKbFromCsvText(text);
      } else if (lowerName.endsWith(".xlsx")) {
        rows = parseKbFromXlsxRows(await parseXlsxFile(file));
      } else {
        throw new Error("Chỉ hỗ trợ file .csv, .json hoặc .xlsx.");
      }
      if (!Array.isArray(rows) || rows.length === 0) {
        throw new Error("File chưa có dòng dữ liệu hợp lệ.");
      }
      renderKbList(rows);
      els.kbUpload.value = "";
      setStatus(`Đã nạp ${rows.length} dòng bộ câu trả lời. Bấm Lưu cấu hình để áp dụng.`, "success");
    } catch (error) {
      setStatus(error.message || "Không thể nhập bộ câu trả lời từ file.", "error");
    }
  }

  async function downloadDocumentsCsvFile() {
    const csvText = buildDocumentsCsvText(documentItems);
    downloadTextFile("documents-export.csv", csvText, "text/csv;charset=utf-8");
  }

  function getDocumentsXlsxRows(items) {
    return (Array.isArray(items) ? items : []).map((row, index) => {
      const source = row && typeof row === "object" ? row : {};
      return {
        id: normalizeText(source.id, `doc_${index + 1}`, 160),
        title: normalizeText(source.title, `Tài liệu ${index + 1}`, 180),
        tags: Array.isArray(source.tags) ? source.tags.join("|") : "",
        text: normalizeText(source.text, "", 120000),
        sourceUrls: Array.isArray(source.sourceUrls) ? source.sourceUrls.join("|") : ""
      };
    });
  }

  function getKnowledgeBaseXlsxRows(items) {
    return (Array.isArray(items) ? items : []).map((row, index) => {
      const source = row && typeof row === "object" ? row : {};
      return {
        id: normalizeText(source.id, `kb_${index + 1}`, 80),
        keywords: Array.isArray(source.keywords) ? source.keywords.join("|") : "",
        answer: normalizeText(source.answer, "", 2000),
        quickReplies: Array.isArray(source.quickReplies) ? source.quickReplies.join("|") : ""
      };
    });
  }

  async function downloadDocumentsXlsxFile() {
    const rows = getDocumentsXlsxRows(documentItems);
    downloadXlsxFile("documents-export.xlsx", rows, "documents");
  }

  async function downloadKnowledgeBaseXlsxFile() {
    const rows = getKnowledgeBaseXlsxRows(collectKnowledgeBase());
    downloadXlsxFile("knowledge-base-export.xlsx", rows, "knowledge_base");
  }

  async function downloadDocumentsTemplateXlsxFile() {
    const rows = [
      {
        id: "doc_login",
        title: "Hướng dẫn đăng nhập",
        tags: "đăng nhập|login|vào tài khoản",
        text: "Bước 1: Mở trang đăng nhập. Bước 2: Nhập tài khoản và mật khẩu. Bước 3: Bấm nút Đăng nhập.",
        sourceUrls: "https://websitecuaban.com/dang-nhap"
      },
      {
        id: "doc_booking",
        title: "Hướng dẫn đặt lịch",
        tags: "đặt lịch|booking|hẹn",
        text: "Bạn chọn dịch vụ, chọn ngày giờ, nhập thông tin liên hệ rồi xác nhận đặt lịch.",
        sourceUrls: "https://websitecuaban.com/dat-lich|https://websitecuaban.com/lich-cua-toi"
      }
    ];
    downloadXlsxFile("documents-template.xlsx", rows, "documents_template");
  }

  async function downloadKnowledgeBaseTemplateXlsxFile() {
    const rows = [
      {
        id: "pricing",
        keywords: "giá|bảng giá|chi phí",
        answer: "Bên mình có nhiều gói theo dịch vụ. Bạn để lại nhu cầu cụ thể, mình sẽ tư vấn gói phù hợp và báo giá chi tiết.",
        quickReplies: "Tư vấn ngay|Đặt lịch"
      },
      {
        id: "booking",
        keywords: "đặt lịch|hẹn lịch|booking",
        answer: "Bạn vui lòng để lại số điện thoại và khung giờ mong muốn, bên mình sẽ xác nhận lịch ngay.",
        quickReplies: "Sáng|Chiều|Tối"
      }
    ];
    downloadXlsxFile("kb-template.xlsx", rows, "kb_template");
  }

  async function downloadKnowledgeBaseCsvFile() {
    const rows = collectKnowledgeBase();
    const csvText = buildKnowledgeBaseCsvText(rows);
    downloadTextFile("knowledge-base-export.csv", csvText, "text/csv;charset=utf-8");
  }

  async function copyEmbedCode(overrideText) {
    const text = typeof overrideText === "string" ? overrideText : (els.embedCode ? els.embedCode.value : "");
    if (!text) {
      return;
    }
    try {
      if (navigator.clipboard && navigator.clipboard.writeText) {
        await navigator.clipboard.writeText(text);
      } else {
        els.embedCode.focus();
        els.embedCode.select();
        document.execCommand("copy");
      }
      setStatus("Đã sao chép mã nhúng.", "success");
    } catch (_) {
      setStatus("Không thể sao chép tự động. Vui lòng sao chép thủ công.", "error");
    }
  }

  function saveAllToBundleStateFromForm() {
    ensureBundleState();
    staticBundle.config = mergeConfig(collectConfigFromForm());
    staticBundle.documents = Array.isArray(documentItems) ? documentItems.slice() : [];
    persistBundleState();
  }

  async function downloadBundleFile() {
    saveAllToBundleStateFromForm();
    const bundle = createBundleForExport();
    const blob = new Blob([JSON.stringify(bundle, null, 2)], { type: "application/json;charset=utf-8" });
    const url = URL.createObjectURL(blob);
    const anchor = document.createElement("a");
    anchor.href = url;
    anchor.download = "widget-data.json";
    document.body.appendChild(anchor);
    anchor.click();
    anchor.remove();
    URL.revokeObjectURL(url);
  }

  async function importBundleFile(file) {
    if (!file) {
      throw new Error("Bạn chưa chọn file JSON.");
    }
    const text = await readFileAsText(file);
    const parsed = parseJsonSafely(text);
    if (!parsed || typeof parsed !== "object") {
      throw new Error("File dữ liệu không hợp lệ.");
    }
    staticBundle = normalizeBundle(parsed);
    const preferredWorkspaceId = normalizeText(currentWorkspace && currentWorkspace.id ? currentWorkspace.id : "", "", 120);
    if (preferredWorkspaceId) {
      const hasPreferredWorkspace = Array.isArray(staticBundle.workspaces)
        && staticBundle.workspaces.some((item) => item && item.id === preferredWorkspaceId);
      if (hasPreferredWorkspace) {
        staticBundle.activeWorkspaceId = preferredWorkspaceId;
      }
    }
    writeBundleCache(staticBundle);
    syncStateFromBundle();
  }

  async function callDirectDeployApi(action) {
    const deployKey = normalizeText(els.directDeployKey ? els.directDeployKey.value : "", "", 240);
    if (!deployKey) {
      throw new Error("Bạn cần nhập khóa deploy trước khi thao tác.");
    }
    writeStoredDeployKey(deployKey);
    const payload = {
      action: normalizeText(action, "ping", 30),
      deployKey
    };
    if (payload.action === "deploy") {
      saveAllToBundleStateFromForm();
      payload.bundle = createBundleForExport();
    }
    const response = await fetch(DIRECT_DEPLOY_ENDPOINT, {
      method: "POST",
      cache: "no-store",
      headers: {
        "Content-Type": "application/json;charset=utf-8"
      },
      body: JSON.stringify(payload)
    });
    const bodyText = await response.text();
    const parsed = parseJsonSafely(bodyText);
    const message = parsed && typeof parsed.message === "string"
      ? normalizeText(parsed.message, "", 500)
      : "";
    if (!response.ok || !parsed || parsed.ok !== true) {
      throw new Error(message || `Không thể deploy trực tiếp (HTTP ${response.status}).`);
    }
    return parsed;
  }

  async function verifyDirectDeployConnection() {
    const payload = await callDirectDeployApi("ping");
    return payload;
  }

  async function deployBundleDirectly() {
    const payload = await callDirectDeployApi("deploy");
    try {
      await loadBundleFromHost();
      syncStateFromBundle();
    } catch (_) {}
    return payload;
  }

  function bindEvents() {
    const tabButtons = getAdminTabButtons();
    tabButtons.forEach((button) => {
      if (!(button instanceof HTMLButtonElement)) {
        return;
      }
      button.addEventListener("click", () => {
        const tabId = normalizeText(button.getAttribute("data-admin-tab"), ADMIN_TAB_IDS[0], 40);
        setActiveAdminTab(tabId);
        setStatus(`Đang mở tab: ${button.textContent || tabId}`, "success");
      });
    });

    if (els.authTabs) {
      els.authTabs.addEventListener("click", (event) => {
        const target = event.target;
        if (!(target instanceof Element)) {
          return;
        }
        const button = target.closest("[data-auth-tab]");
        if (!(button instanceof HTMLButtonElement)) {
          return;
        }
        const tabId = normalizeText(button.getAttribute("data-auth-tab"), AUTH_TAB_IDS[0], 40);
        setActiveAuthTab(tabId);
      });
    }

    if (els.authRegisterSubmit) {
      els.authRegisterSubmit.addEventListener("click", () => {
        setAuthStatus("Đang đăng ký tài khoản...", "muted");
        registerWorkspaceAccount()
          .then((payload) => {
            const username = normalizeText(payload && payload.account ? payload.account.username : "", "", 120);
            if (els.authLoginUser && username) {
              els.authLoginUser.value = username;
            }
            if (els.authLoginPassword && els.authRegisterPassword) {
              els.authLoginPassword.value = normalizeText(els.authRegisterPassword.value, "", 200);
            }
            if (els.authRegisterUser) {
              els.authRegisterUser.value = "";
            }
            if (els.authRegisterPassword) {
              els.authRegisterPassword.value = "";
            }
            setActiveAuthTab("login");
            setAuthStatus("Đăng ký thành công. Bạn hãy bấm Đăng nhập để vào hệ thống.", "success");
          })
          .catch((error) => setAuthStatus(error.message || "Không thể đăng ký tài khoản.", "error"));
      });
    }

    if (els.authLoginSubmit) {
      els.authLoginSubmit.addEventListener("click", () => {
        setAuthStatus("Đang đăng nhập...", "muted");
        loginWorkspaceAccount()
          .then((payload) => {
            applyAuthSessionPayload(payload);
            setAuthStatus("Đăng nhập thành công.", "success");
            return initializeAdminAfterAuth();
          })
          .catch((error) => setAuthStatus(error.message || "Đăng nhập thất bại.", "error"));
      });
    }

    if (els.authLogout) {
      els.authLogout.addEventListener("click", () => {
        logoutWorkspaceAccount().catch(() => {
          setAuthStatus("Đăng xuất thất bại. Vui lòng tải lại trang.", "error");
        });
      });
    }

    if (els.workspaceCreateName && els.workspaceCreateId) {
      els.workspaceCreateName.addEventListener("input", () => {
        const idValue = normalizeText(els.workspaceCreateId.value, "", 120);
        if (idValue) {
          return;
        }
        const generated = slugify(els.workspaceCreateName.value, "");
        if (generated) {
          els.workspaceCreateId.value = generated;
        }
      });
    }

    if (els.workspaceCreateSubmit) {
      els.workspaceCreateSubmit.addEventListener("click", () => {
        setStatus("Đang tạo workspace mới...", "muted");
        createWorkspaceForCurrentUser()
          .then(() => initializeWorkspaceContext())
          .then((hasWorkspace) => {
            if (els.workspaceCreateName) {
              els.workspaceCreateName.value = "";
            }
            if (els.workspaceCreateId) {
              els.workspaceCreateId.value = "";
            }
            if (hasWorkspace) {
              setQuickStatus("Workspace đã sẵn sàng. Bạn có thể bắt đầu cấu hình ngay.", "success");
            }
            setStatus("Đã tạo workspace mới và mở sẵn để cấu hình.", "success");
          })
          .catch((error) => setStatus(error.message || "Không thể tạo workspace.", "error"));
      });
    }

    if (els.workspaceSwitchSubmit) {
      els.workspaceSwitchSubmit.addEventListener("click", () => {
        const workspaceId = normalizeText(
          els.workspaceSwitchSelect && "value" in els.workspaceSwitchSelect ? els.workspaceSwitchSelect.value : "",
          "",
          120
        );
        if (!workspaceId) {
          setStatus("Bạn chưa có workspace nào để mở.", "error");
          return;
        }
        setStatus("Đang mở workspace...", "muted");
        openWorkspaceForCurrentUser(workspaceId)
          .then(() => initializeWorkspaceContext())
          .then((hasWorkspace) => {
            if (hasWorkspace) {
              setQuickStatus("Đã chuyển workspace thành công.", "success");
            }
            setStatus(`Đã mở workspace: ${workspaceId}`, "success");
          })
          .catch((error) => setStatus(error.message || "Không thể mở workspace.", "error"));
      });
    }

    if (els.superRefreshAccounts) {
      els.superRefreshAccounts.addEventListener("click", () => {
        renderSuperAdminAccountsPanel();
        setStatus("Đã tải lại danh sách tài khoản.", "success");
      });
    }

    if (els.superAccountsList) {
      els.superAccountsList.addEventListener("click", (event) => {
        const target = event.target;
        if (!(target instanceof Element)) {
          return;
        }
        const toggleButton = target.closest(".super-toggle-lock");
        if (!toggleButton) {
          return;
        }
        if (currentUserRole !== "superadmin") {
          setStatus("Chỉ Super Admin mới có quyền này.", "error");
          return;
        }
        const accountId = normalizeText(toggleButton.getAttribute("data-account-id"), "", 120);
        if (!accountId) {
          return;
        }
        const nextLocked = normalizeText(toggleButton.getAttribute("data-next-locked"), "0", 1) === "1";
        const actionText = nextLocked ? "khóa" : "mở khóa";
        setStatus(`Đang ${actionText} tài khoản...`, "muted");
        Promise.resolve()
          .then(() => setAccountLockedState(accountId, nextLocked))
          .then(() => {
            renderSuperAdminAccountsPanel();
            setStatus(
              nextLocked
                ? "Đã khóa tài khoản. Tài khoản này sẽ không thể đăng nhập."
                : "Đã mở khóa tài khoản. Tài khoản này có thể đăng nhập lại.",
              "success"
            );
          })
          .catch((error) => setStatus(error.message || "Không thể cập nhật trạng thái tài khoản.", "error"));
      });
    }

    if (els.quickSiteUrl) {
      els.quickSiteUrl.addEventListener("input", () => {
        renderQuickEmbedPreview();
      });
      els.quickSiteUrl.addEventListener("change", () => {
        const normalized = normalizeUrlInput(els.quickSiteUrl.value);
        if (normalized) {
          els.quickSiteUrl.value = normalized;
        }
        if (els.quickLoginUrl) {
          const hasManualLoginUrl = Boolean(normalizeText(els.quickLoginUrl.value, "", 400));
          if (!hasManualLoginUrl) {
            els.quickLoginUrl.value = buildDefaultLoginUrl(els.quickSiteUrl.value);
          }
        }
        renderQuickEmbedPreview();
      });
    }

    if (els.quickAutoDeploy) {
      els.quickAutoDeploy.addEventListener("click", () => {
        setStatus("Đang lưu thiết lập nhanh...", "muted");
        runQuickAutoDeploy().catch((error) => {
          setQuickStatus(error.message || "Lưu thiết lập nhanh thất bại.", "error");
          setStatus(error.message || "Lưu thiết lập nhanh thất bại.", "error");
        });
      });
    }

    if (els.quickTogglePassword && els.quickLoginPassword) {
      els.quickTogglePassword.setAttribute("aria-label", "Hiện mật khẩu");
      els.quickTogglePassword.addEventListener("click", () => {
        const isPassword = els.quickLoginPassword.type === "password";
        els.quickLoginPassword.type = isPassword ? "text" : "password";
        els.quickTogglePassword.textContent = isPassword ? "Ẩn" : "Hiện";
        els.quickTogglePassword.setAttribute("aria-label", isPassword ? "Ẩn mật khẩu" : "Hiện mật khẩu");
        setStatus(isPassword ? "Đã bật hiển thị mật khẩu." : "Đã ẩn mật khẩu.", "success");
      });
    }

    if (els.quickCopyEmbed) {
      els.quickCopyEmbed.addEventListener("click", () => {
        copyEmbedCode(els.quickEmbedCode ? els.quickEmbedCode.value : "");
      });
    }

    if (els.quickOpenDemo) {
      els.quickOpenDemo.addEventListener("click", () => {
        const demoUrl = buildDemoUrl(getPlatformProjectId());
        window.open(demoUrl, "_blank", "noopener");
        setStatus("Đã mở demo ở tab mới.", "success");
      });
    }

    if (els.bundleDownload) {
      els.bundleDownload.addEventListener("click", () => {
        setStatus("Đang tạo file deploy...", "muted");
        downloadBundleFile()
          .then(() => setStatus("Đã tải file widget-data.json. Upload file này lên host để áp dụng.", "success"))
          .catch((error) => setStatus(error.message || "Không thể tải file deploy.", "error"));
      });
    }

    if (els.bundleImport) {
      els.bundleImport.addEventListener("click", () => {
        const file = els.bundleUpload && els.bundleUpload.files && els.bundleUpload.files[0]
          ? els.bundleUpload.files[0]
          : null;
        setStatus("Đang nạp file dữ liệu...", "muted");
        importBundleFile(file)
          .then(() => {
            if (els.bundleUpload) {
              els.bundleUpload.value = "";
            }
            setStatus("Đã nạp file dữ liệu thành công.", "success");
          })
          .catch((error) => setStatus(error.message || "Nạp file dữ liệu thất bại.", "error"));
      });
    }

    if (els.directToggleDeployKey && els.directDeployKey) {
      els.directToggleDeployKey.setAttribute("aria-label", "Hiện khóa deploy");
      els.directToggleDeployKey.addEventListener("click", () => {
        const isPassword = els.directDeployKey.type === "password";
        els.directDeployKey.type = isPassword ? "text" : "password";
        els.directToggleDeployKey.textContent = isPassword ? "Ẩn" : "Hiện";
        els.directToggleDeployKey.setAttribute("aria-label", isPassword ? "Ẩn khóa deploy" : "Hiện khóa deploy");
        setDirectDeployStatus(isPassword ? "Đã bật hiển thị khóa deploy." : "Đã ẩn khóa deploy.", "success");
      });
    }

    if (els.directDeployTest) {
      els.directDeployTest.addEventListener("click", () => {
        setDirectDeployStatus("Đang kiểm tra kết nối deploy...", "muted");
        setStatus("Đang kiểm tra endpoint deploy trực tiếp...", "muted");
        verifyDirectDeployConnection()
          .then((payload) => {
            const message = normalizeText(payload && payload.message, "Kết nối deploy thành công.", 260);
            setDirectDeployStatus(message, "success");
            setStatus(message, "success");
          })
          .catch((error) => {
            const message = error && error.message ? error.message : "Kiểm tra deploy thất bại.";
            setDirectDeployStatus(message, "error");
            setStatus(message, "error");
          });
      });
    }

    if (els.directDeploySubmit) {
      els.directDeploySubmit.addEventListener("click", () => {
        setDirectDeployStatus("Đang lưu trực tiếp dữ liệu lên host...", "muted");
        setStatus("Đang deploy trực tiếp lên host...", "muted");
        deployBundleDirectly()
          .then((payload) => {
            const message = normalizeText(payload && payload.message, "Đã deploy trực tiếp thành công.", 260);
            setDirectDeployStatus(message, "success");
            setQuickStatus("Đã deploy trực tiếp lên host. Widget đang dùng dữ liệu mới nhất.", "success");
            setStatus(message, "success");
          })
          .catch((error) => {
            const message = error && error.message ? error.message : "Deploy trực tiếp thất bại.";
            setDirectDeployStatus(message, "error");
            setStatus(message, "error");
          });
      });
    }

    els.reloadConfig.addEventListener("click", () => {
      setStatus("Đang tải dữ liệu từ file trên host...", "muted");
      loadBundleFromHost()
        .then(() => {
          syncStateFromBundle();
          setStatus("Đã tải dữ liệu từ host.", "success");
        })
        .catch((error) => {
          setStatus(error.message || "Không thể tải dữ liệu từ host.", "error");
        });
    });

    els.saveConfig.addEventListener("click", () => {
      saveConfig()
        .then(() => setStatus("Đã lưu tạm trên trình duyệt. Nhớ bấm “Tải file deploy” để upload lên host.", "success"))
        .catch((error) => {
          setStatus(error.message || "Lưu cấu hình thất bại.", "error");
        });
    });

    if (els.advancedSaveConfig) {
      els.advancedSaveConfig.addEventListener("click", () => {
        saveConfig()
          .then(() => setStatus("Đã lưu cài đặt nâng cao trên trình duyệt.", "success"))
          .catch((error) => {
            setStatus(error.message || "Lưu cài đặt nâng cao thất bại.", "error");
          });
      });
    }

    els.reloadDocuments.addEventListener("click", () => {
      setStatus("Đang tải tài liệu...", "muted");
      loadDocuments()
        .then(() => setStatus("Đã tải danh sách tài liệu.", "success"))
        .catch((error) => {
          setStatus(error.message || "Tải tài liệu thất bại.", "error");
        });
    });

    if (els.documentsSearch) {
      els.documentsSearch.addEventListener("input", () => {
        documentsSearchQuery = normalizeText(els.documentsSearch.value, "", 240);
        documentsCurrentPage = 1;
        renderDocumentListPage();
      });
    }

    if (els.documentsClearSearch) {
      els.documentsClearSearch.addEventListener("click", () => {
        documentsSearchQuery = "";
        documentsCurrentPage = 1;
        if (els.documentsSearch) {
          els.documentsSearch.value = "";
        }
        renderDocumentListPage();
        setStatus("Đã xóa bộ lọc tìm kiếm tài liệu.", "success");
      });
    }

    if (els.documentsPageSize) {
      els.documentsPageSize.addEventListener("change", () => {
        documentsPageSize = normalizeDocumentsPageSize(els.documentsPageSize.value);
        documentsCurrentPage = 1;
        renderDocumentListPage();
        setStatus(`Đang hiển thị ${documentsPageSize} tài liệu mỗi trang.`, "success");
      });
    }

    if (els.documentsPagePrev) {
      els.documentsPagePrev.addEventListener("click", () => {
        documentsCurrentPage = Math.max(1, documentsCurrentPage - 1);
        renderDocumentListPage();
      });
    }

    if (els.documentsPageNext) {
      els.documentsPageNext.addEventListener("click", () => {
        documentsCurrentPage += 1;
        renderDocumentListPage();
      });
    }

    if (els.documentsExportCsv) {
      els.documentsExportCsv.addEventListener("click", () => {
        downloadDocumentsCsvFile()
          .then(() => setStatus("Đã tải file CSV tài liệu hiện tại.", "success"))
          .catch((error) => setStatus(error.message || "Không thể tải CSV tài liệu.", "error"));
      });
    }

    if (els.documentsTemplateXlsx) {
      els.documentsTemplateXlsx.addEventListener("click", () => {
        downloadDocumentsTemplateXlsxFile()
          .then(() => setStatus("Đã tải file mẫu tài liệu XLSX.", "success"))
          .catch((error) => setStatus(error.message || "Không thể tải file mẫu XLSX.", "error"));
      });
    }

    if (els.documentsExportXlsx) {
      els.documentsExportXlsx.addEventListener("click", () => {
        downloadDocumentsXlsxFile()
          .then(() => setStatus("Đã tải file XLSX tài liệu hiện tại.", "success"))
          .catch((error) => setStatus(error.message || "Không thể tải XLSX tài liệu.", "error"));
      });
    }

    if (els.platformLoadProject) {
      els.platformLoadProject.addEventListener("click", () => {
        setStatus("Đang tải thông tin website...", "muted");
        loadPlatformProject()
          .then(() => setStatus("Đã tải thông tin website.", "success"))
          .catch((error) => setStatus(error.message || "Tải website thất bại.", "error"));
      });
    }

    if (els.platformSaveProject) {
      els.platformSaveProject.addEventListener("click", () => {
        setStatus("Đang lưu website...", "muted");
        savePlatformProject()
          .then(() => setStatus("Đã lưu website.", "success"))
          .catch((error) => setStatus(error.message || "Lưu website thất bại.", "error"));
      });
    }

    if (els.platformRunCrawl) {
      els.platformRunCrawl.addEventListener("click", () => {
        setStatus("Đang quét website và tạo hướng dẫn...", "muted");
        runPlatformCrawl()
          .then((data) => {
            const guides =
              data && data.crawl && data.crawl.summary ? data.crawl.summary.generatedGuides || 0 : 0;
            setStatus(`Quét xong. Đã sinh ${guides} hướng dẫn.`, "success");
          })
          .catch((error) => setStatus(error.message || "Quét website thất bại.", "error"));
      });
    }

    if (els.platformPublishGuides) {
      els.platformPublishGuides.addEventListener("click", () => {
        setStatus("Đang lưu bản duyệt và xuất bản vào chatbot...", "muted");
        publishReviewedGuides()
          .then((data) => {
            const published = data && data.published ? data.published : {};
            setStatus(
              `Đã đưa ${published.documents || 0} tài liệu, ${published.knowledgeBase || 0} câu trả lời.`,
              "success"
            );
          })
          .catch((error) => setStatus(error.message || "Đưa hướng dẫn vào chatbot thất bại.", "error"));
      });
    }

    if (els.platformProjectId) {
      els.platformProjectId.addEventListener("input", () => {
        renderEmbedCode();
        renderQuickEmbedPreview();
      });
      els.platformProjectId.addEventListener("change", () => {
        renderEmbedCode();
        renderQuickEmbedPreview();
      });
    }

    if (els.reviewLoadDraft) {
      els.reviewLoadDraft.addEventListener("click", () => {
        setStatus("Đang nạp bản nháp cần duyệt...", "muted");
        loadReviewDraft()
          .then(() => setStatus("Đã nạp bản nháp để duyệt.", "success"))
          .catch((error) => setStatus(error.message || "Nạp bản nháp thất bại.", "error"));
      });
    }

    if (els.reviewCheckAll) {
      els.reviewCheckAll.addEventListener("click", () => {
        if (!els.reviewGuidesList) {
          return;
        }
        const checks = Array.from(els.reviewGuidesList.querySelectorAll(".review-approved"));
        checks.forEach((row) => {
          if (row instanceof HTMLInputElement) {
            row.checked = true;
          }
        });
        const guides = collectReviewDraftFromForm();
        renderReviewSummary({
          projectId: getPlatformProjectId(),
          guides,
          totals: computeReviewTotals(guides),
          crawlStatus: currentCrawlStatus
        });
        setStatus("Đã đánh dấu duyệt toàn bộ bản nháp hiện tại.", "success");
      });
    }

    if (els.reviewSaveDraft) {
      els.reviewSaveDraft.addEventListener("click", () => {
        setStatus("Đang lưu bản nháp đã chỉnh...", "muted");
        saveReviewDraft()
          .then(() => setStatus("Đã lưu bản nháp duyệt.", "success"))
          .catch((error) => setStatus(error.message || "Lưu bản nháp thất bại.", "error"));
      });
    }

    if (els.reviewPublishGuides) {
      els.reviewPublishGuides.addEventListener("click", () => {
        setStatus("Đang xuất bản nội dung đã duyệt...", "muted");
        publishReviewedGuides()
          .then((data) => {
            const published = data && data.published ? data.published : {};
            setStatus(
              `Đã xuất bản ${published.documents || 0} tài liệu và ${published.knowledgeBase || 0} câu trả lời.`,
              "success"
            );
          })
          .catch((error) => setStatus(error.message || "Xuất bản thất bại.", "error"));
      });
    }

    if (els.reviewGuidesList) {
      const refreshReview = () => {
        const guides = collectReviewDraftFromForm();
        renderReviewSummary({
          projectId: getPlatformProjectId(),
          guides,
          totals: computeReviewTotals(guides),
          crawlStatus: currentCrawlStatus
        });
      };
      els.reviewGuidesList.addEventListener("change", refreshReview);
      els.reviewGuidesList.addEventListener("input", (event) => {
        const target = event.target;
        if (!(target instanceof Element)) {
          return;
        }
        if (
          target.classList.contains("review-approved") ||
          target.classList.contains("review-title") ||
          target.classList.contains("review-keywords") ||
          target.classList.contains("review-replies")
        ) {
          refreshReview();
        }
      });
    }

    els.addDocument.addEventListener("click", () => {
      handleManualDocumentAdd();
    });

    els.uploadDocuments.addEventListener("click", () => {
      handleFileUpload();
    });

    els.documentsList.addEventListener("click", (event) => {
      const target = event.target;
      if (!(target instanceof Element)) {
        return;
      }
      const removeButton = target.closest(".remove-document");
      if (removeButton) {
        const id = removeButton.getAttribute("data-id") || "";
        if (!id) {
          return;
        }
        deleteDocument(id)
          .then(() => setStatus(`Đã xóa tài liệu: ${id}`, "success"))
          .catch((error) => setStatus(error.message || "Xóa tài liệu thất bại.", "error"));
        return;
      }

      const toggleButton = target.closest(".toggle-edit-document");
      if (toggleButton) {
        const item = toggleButton.closest(".doc-item");
        const editPanel = item ? item.querySelector(".doc-edit") : null;
        if (!(editPanel instanceof HTMLElement)) {
          return;
        }
        const isHidden = editPanel.hasAttribute("hidden");
        if (isHidden) {
          editPanel.removeAttribute("hidden");
          toggleButton.textContent = "Thu gọn";
        } else {
          editPanel.setAttribute("hidden", "");
          toggleButton.textContent = "Sửa";
        }
        return;
      }

      const cancelButton = target.closest(".cancel-document-edit");
      if (cancelButton) {
        const item = cancelButton.closest(".doc-item");
        const editPanel = item ? item.querySelector(".doc-edit") : null;
        const toggle = item ? item.querySelector(".toggle-edit-document") : null;
        if (editPanel instanceof HTMLElement) {
          editPanel.setAttribute("hidden", "");
        }
        if (toggle instanceof HTMLElement) {
          toggle.textContent = "Sửa";
        }
        return;
      }

      const saveButton = target.closest(".save-document-edit");
      if (saveButton) {
        const id = saveButton.getAttribute("data-id") || "";
        if (!id) {
          return;
        }
        const item = saveButton.closest(".doc-item");
        if (!item) {
          return;
        }
        const titleInput = item.querySelector(".doc-edit-title");
        const tagsInput = item.querySelector(".doc-edit-tags");
        const textInput = item.querySelector(".doc-edit-text");
        const title = normalizeText(titleInput && "value" in titleInput ? titleInput.value : "", "Không tiêu đề", 160);
        const text = normalizeText(textInput && "value" in textInput ? textInput.value : "", "", 120000);
        const tags = parseCsv(tagsInput && "value" in tagsInput ? tagsInput.value : "");
        if (!text) {
          setStatus("Nội dung tài liệu không được để trống.", "error");
          return;
        }
        updateDocument(id, { title, text, tags })
          .then(() => setStatus(`Đã cập nhật tài liệu: ${id}`, "success"))
          .catch((error) => setStatus(error.message || "Cập nhật tài liệu thất bại.", "error"));
      }
    });

    els.addKbRow.addEventListener("click", () => {
      els.kbList.appendChild(createKbRow({ keywords: [], answer: "", quickReplies: [] }));
      setStatus("Đã thêm một dòng bộ câu trả lời mẫu.", "success");
    });

    if (els.kbImport) {
      els.kbImport.addEventListener("click", () => {
        importKnowledgeBaseFromFile();
      });
    }

    if (els.kbExportCsv) {
      els.kbExportCsv.addEventListener("click", () => {
        downloadKnowledgeBaseCsvFile()
          .then(() => setStatus("Đã tải file CSV bộ câu trả lời hiện tại.", "success"))
          .catch((error) => setStatus(error.message || "Không thể tải CSV bộ câu trả lời.", "error"));
      });
    }

    if (els.kbTemplateXlsx) {
      els.kbTemplateXlsx.addEventListener("click", () => {
        downloadKnowledgeBaseTemplateXlsxFile()
          .then(() => setStatus("Đã tải file mẫu bộ câu trả lời XLSX.", "success"))
          .catch((error) => setStatus(error.message || "Không thể tải file mẫu XLSX.", "error"));
      });
    }

    if (els.kbExportXlsx) {
      els.kbExportXlsx.addEventListener("click", () => {
        downloadKnowledgeBaseXlsxFile()
          .then(() => setStatus("Đã tải file XLSX bộ câu trả lời hiện tại.", "success"))
          .catch((error) => setStatus(error.message || "Không thể tải XLSX bộ câu trả lời.", "error"));
      });
    }

    els.logoUpload.addEventListener("change", () => {
      const file = els.logoUpload.files && els.logoUpload.files[0] ? els.logoUpload.files[0] : null;
      handleLogoFile(file);
      els.logoUpload.value = "";
    });

    els.logoClear.addEventListener("click", () => {
      currentLogoDataUrl = "";
      renderLogoPreview("");
      setStatus("Đã xóa logo. Bấm Lưu cấu hình để áp dụng.", "muted");
    });

    els.copyEmbed.addEventListener("click", () => {
      copyEmbedCode();
    });

    document.addEventListener("click", (event) => {
      const target = event.target;
      if (!(target instanceof Element)) {
        return;
      }
      const button = target.closest("button");
      if (!(button instanceof HTMLButtonElement)) {
        return;
      }
      const label = normalizeText(button.textContent || "", "Thao tác", 80);
      const message = `Đã nhận thao tác: ${label}.`;
      if (button.closest("#auth-gate")) {
        setAuthStatus(message, "muted");
      } else {
        setStatus(message, "muted");
      }
    }, true);
  }

  function isAuthError(error) {
    const message = normalizeText(error && error.message ? error.message : "", "", 400).toLowerCase();
    if (!message) {
      return false;
    }
    return (
      message.includes("http 401") ||
      message.includes("auth_required") ||
      message.includes("dang nhap") ||
      message.includes("đăng nhập")
    );
  }

  async function initializeWorkspaceContext() {
    renderWorkspaceToolsPanel();
    renderSuperAdminAccountsPanel();
    updateRoleBasedAdminLayout();
    if (currentUserRole === "superadmin") {
      setWorkspaceDependentVisible(true);
      renderSuperAdminAccountsPanel();
      setStatus("Super Admin chỉ quản lý tài khoản: khóa/mở khóa tài khoản thường.", "success");
      setQuickStatus("Chế độ Super Admin không có quyền tạo hoặc chỉnh widget.", "muted");
      return true;
    }
    const currentAccount = getCurrentAccount();
    if (currentUserRole === "user" && currentAccount && isAccountLocked(currentAccount)) {
      await logoutWorkspaceAccount();
      setAuthStatus("Tài khoản của bạn đang bị khóa. Vui lòng liên hệ Super Admin.", "error");
      return false;
    }
    setActiveAdminTab(activeAdminTab, { persist: false });
    if (!currentWorkspace || !currentWorkspace.id) {
      setWorkspaceDependentVisible(false);
      setQuickStatus("Tài khoản chưa có workspace. Hãy tạo workspace đầu tiên ở phần Workspace của bạn.", "error");
      setStatus("Tài khoản đã đăng nhập nhưng chưa có workspace.", "muted");
      renderEmbedCode();
      renderQuickEmbedPreview();
      return false;
    }
    setWorkspaceDependentVisible(true);
    await Promise.all([loadConfig(), loadDocuments(), loadPlatformProject()]);
    renderWorkspaceToolsPanel();
    renderSuperAdminAccountsPanel();
    syncQuickFormFromMain();
    return true;
  }

  async function initializeAdminAfterAuth() {
    setAuthenticatedView(true);
    if (currentUserRole === "superadmin") {
      setStatus("Đang tải dữ liệu tài khoản...", "muted");
    } else {
      setStatus("Đang tải dữ liệu workspace...", "muted");
    }
    const hasWorkspace = await initializeWorkspaceContext();
    if (!hasWorkspace) {
      return;
    }
    if (currentUserRole === "superadmin") {
      setStatus("Sẵn sàng. Bạn có thể khóa/mở khóa tài khoản thường.", "success");
      return;
    }
    setQuickStatus("Sẵn sàng. Điền thông tin rồi bấm “Lưu thiết lập nhanh”.", "success");
    setStatus("Sẵn sàng.", "success");
  }

  async function bootstrap() {
    initializeCardCollapsers();
    bindEvents();
    setActiveAuthTab(activeAuthTab);
    activeAdminTab = readStoredAdminTab();
    setActiveAdminTab(activeAdminTab, { persist: false });
    const rememberedDeployKey = readStoredDeployKey();
    if (els.directDeployKey && rememberedDeployKey && !normalizeText(els.directDeployKey.value, "", 240)) {
      els.directDeployKey.value = rememberedDeployKey;
    }
    setDirectDeployStatus("Nhập khóa deploy rồi bấm “Kiểm tra kết nối deploy” trước khi lưu trực tiếp.", "muted");
    setAuthenticatedView(false);
    setAuthStatus("Đang tải dữ liệu hệ thống...", "muted");
    let loadedSource = "default";
    if (window.location.protocol === "file:") {
      const header = document.querySelector(".page-header");
      if (header) {
        const notice = document.createElement("div");
        notice.className = "runtime-notice";
        notice.textContent =
          "Bạn đang mở bằng file://. Một số trình duyệt sẽ chặn đọc file JSON tự động. Vẫn có thể nạp dữ liệu bằng nút “Nạp file dữ liệu”.";
        header.appendChild(notice);
      }
      setStatus("Đang ở chế độ xem file cục bộ.", "muted");
    }

    const cachedBundle = readBundleCache();
    let hostBundle = null;
    try {
      hostBundle = await fetchBundleFromHost();
      loadedSource = "host";
    } catch (_) {
      hostBundle = null;
    }
    if (hostBundle && cachedBundle) {
      if (shouldPreferCacheBundle(cachedBundle, hostBundle)) {
        staticBundle = cachedBundle;
        loadedSource = "cache";
      } else {
        staticBundle = hostBundle;
        writeBundleCache(staticBundle);
        loadedSource = "host";
      }
    } else if (hostBundle) {
      staticBundle = hostBundle;
      writeBundleCache(staticBundle);
      loadedSource = "host";
    } else if (cachedBundle) {
      staticBundle = cachedBundle;
      loadedSource = "cache";
    } else {
      staticBundle = buildDefaultBundle();
      loadedSource = "default";
    }
    syncStateFromBundle();
    const hasSession = await restoreWorkspaceSession();
    if (!hasSession) {
      setAuthenticatedView(false);
      if (loadedSource === "cache") {
        setQuickStatus("Đang dùng dữ liệu cache cục bộ. Khi xong nhớ tải file deploy để upload lên host.", "error");
      }
      setAuthStatus("Vui lòng đăng nhập tài khoản thường hoặc Super Admin để quản trị.", "muted");
      setStatus("Đã nạp dữ liệu. Chờ đăng nhập để bắt đầu.", "muted");
      return;
    }
    await initializeAdminAfterAuth();
    if (loadedSource === "host") {
      setStatus("Đã tải dữ liệu từ file trên host.", "success");
      return;
    }
    if (loadedSource === "cache") {
      setStatus("Đang dùng dữ liệu cache cục bộ mới hơn hoặc chưa đồng bộ lên host.", "error");
      setQuickStatus("Bạn đang dùng dữ liệu cache cục bộ. Nhớ tải file deploy và upload lên host để đồng bộ chính thức.", "error");
      return;
    }
    setStatus("Đang dùng cấu hình mặc định. Hãy chỉnh sửa rồi tải file deploy để triển khai.", "muted");
    setQuickStatus("Bạn đang ở bộ dữ liệu mặc định. Sau khi chỉnh xong, bấm “Tải file deploy”.", "error");
  }

  bootstrap().catch((error) => {
    setStatus(error.message || "Khởi tạo thất bại.", "error");
    setQuickStatus(error.message || "Khởi tạo thất bại. Vui lòng tải lại trang.", "error");
    setAuthStatus(error.message || "Khởi tạo thất bại.", "error");
    renderConfig(DEFAULT_CONFIG);
    renderDocuments([]);
  });
})();
