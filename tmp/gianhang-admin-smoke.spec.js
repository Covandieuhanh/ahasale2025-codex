const { test, expect } = require('@playwright/test');

test.use({ ignoreHTTPSErrors: true });

test('admin login smoke', async ({ page }) => {
  await page.goto('https://ahasale.local/gianhang/admin/login.aspx', { waitUntil: 'networkidle' });
  const branch = page.locator("select[id$='ddl_chinhanh']");
  const options = await branch.locator('option').evaluateAll(nodes => nodes.map(n => ({ value: n.value, text: (n.textContent || '').trim() })));
  const chosen = options.find(o => o.value);
  if (!chosen) throw new Error('No branch option available');
  await branch.selectOption(chosen.value);
  await page.locator("input[id$='txt_user']").fill('admin');
  await page.locator("input[id$='txt_pass']").fill('123456');
  await page.locator("input[id$='but_login']").click();
  await page.waitForURL(url => !url.pathname.endsWith('/gianhang/admin/login.aspx'), { timeout: 30000 });
  await expect(page).toHaveURL(/\/gianhang\/admin/);
});
