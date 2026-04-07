(function () {
  "use strict";

  if (window.__AHA_STANDALONE_WIDGET_LOADED__) {
    return;
  }
  window.__AHA_STANDALONE_WIDGET_LOADED__ = true;

  function findCurrentScript() {
    if (document.currentScript) {
      return document.currentScript;
    }
    const scripts = Array.from(document.getElementsByTagName("script"));
    for (let index = scripts.length - 1; index >= 0; index -= 1) {
      const element = scripts[index];
      if (/\/widget\/embed\.js(?:\?|#|$)/i.test(element.src || "")) {
        return element;
      }
    }
    return null;
  }

  const scriptElement = findCurrentScript();
  if (!scriptElement) {
    return;
  }

  function resolveBaseUrl(script) {
    const explicit = (script.getAttribute("data-base-url") || "").trim();
    if (explicit) {
      return explicit.replace(/\/+$/, "");
    }
    try {
      const url = new URL(script.src, window.location.href);
      const rootPath = url.pathname.replace(/\/widget\/embed\.js(?:\?.*)?(?:#.*)?$/i, "");
      return `${url.origin}${rootPath}`.replace(/\/+$/, "");
    } catch (_) {
      return "";
    }
  }

  function normalizeText(value, fallback, maxLength) {
    if (typeof value !== "string") {
      return fallback;
    }
    const text = value.trim();
    if (!text) {
      return fallback;
    }
    return text.slice(0, maxLength);
  }

  function normalizeColor(value, fallback) {
    if (typeof value !== "string") {
      return fallback;
    }
    const text = value.trim();
    if (/^#[0-9a-fA-F]{3}$/.test(text) || /^#[0-9a-fA-F]{6}$/.test(text)) {
      return text;
    }
    return fallback;
  }

  function normalizeNumber(value, fallback, min, max) {
    const parsed = Number(value);
    if (!Number.isFinite(parsed)) {
      return fallback;
    }
    return Math.min(max, Math.max(min, Math.round(parsed)));
  }

  function normalizeBool(value, fallback) {
    if (typeof value === "boolean") {
      return value;
    }
    if (typeof value === "string") {
      const text = value.trim().toLowerCase();
      if (text === "true") {
        return true;
      }
      if (text === "false") {
        return false;
      }
    }
    return fallback;
  }

  function normalizeArray(input, maxItems, itemLength) {
    if (!Array.isArray(input)) {
      return [];
    }
    const output = [];
    const seen = new Set();
    for (let i = 0; i < input.length; i += 1) {
      const text = normalizeText(input[i], "", itemLength);
      if (!text) {
        continue;
      }
      const key = text.toLowerCase();
      if (seen.has(key)) {
        continue;
      }
      seen.add(key);
      output.push(text);
      if (output.length >= maxItems) {
        break;
      }
    }
    return output;
  }

  function normalizeConfig(raw, options) {
    const opts = options && typeof options === "object" ? options : {};
    const root = raw && typeof raw === "object" ? raw : {};
    const widgetKey = normalizeText(opts.widgetKey, "", 120);
    let selectedWorkspace = null;
    if (Array.isArray(root.workspaces)) {
      if (widgetKey) {
        selectedWorkspace = root.workspaces.find((item) => {
          return normalizeText(item && item.widgetKey, "", 120) === widgetKey;
        }) || null;
      }
      if (!selectedWorkspace) {
        const activeWorkspaceId = normalizeText(root.activeWorkspaceId, "", 120);
        selectedWorkspace = root.workspaces.find((item) => normalizeText(item && item.id, "", 120) === activeWorkspaceId) || null;
      }
      if (!selectedWorkspace && root.workspaces.length > 0) {
        selectedWorkspace = root.workspaces[0];
      }
    }
    const workspaceData = selectedWorkspace && selectedWorkspace.data && typeof selectedWorkspace.data === "object"
      ? selectedWorkspace.data
      : {};
    const source = workspaceData.config && typeof workspaceData.config === "object"
      ? workspaceData.config
      : root.config && typeof root.config === "object"
        ? root.config
        : workspaceData && Object.keys(workspaceData).length > 0
          ? workspaceData
          : root;
    const brand = source.brand && typeof source.brand === "object" ? source.brand : {};
    const theme = source.theme && typeof source.theme === "object" ? source.theme : {};
    const layout = source.layout && typeof source.layout === "object" ? source.layout : {};
    const behavior = source.behavior && typeof source.behavior === "object" ? source.behavior : {};
    const rows = Array.isArray(source.knowledgeBase) ? source.knowledgeBase : [];
    const docsRaw = Array.isArray(workspaceData.documents)
      ? workspaceData.documents
      : Array.isArray(root.documents)
        ? root.documents
        : Array.isArray(source.documents)
          ? source.documents
          : [];

    const knowledgeBase = rows
      .map((row, index) => {
        const obj = row && typeof row === "object" ? row : {};
        return {
          id: normalizeText(obj.id, `kb_${index + 1}`, 80),
          keywords: normalizeArray(obj.keywords, 30, 100),
          answer: normalizeText(obj.answer, "", 2000),
          quickReplies: normalizeArray(obj.quickReplies, 8, 80)
        };
      })
      .filter((row) => row.answer);

    const documents = docsRaw
      .map((row, index) => {
        const obj = row && typeof row === "object" ? row : {};
        const text = normalizeText(obj.text, "", 120000);
        if (!text) {
          return null;
        }
        return {
          id: normalizeText(obj.id, `doc_${index + 1}`, 120),
          title: normalizeText(obj.title, "Tài liệu", 180),
          text,
          tags: normalizeArray(obj.tags, 20, 50),
          sourceUrls: Array.isArray(obj && obj.meta && obj.meta.sourceUrls)
            ? normalizeArray(obj.meta.sourceUrls, 4, 500)
            : normalizeArray(obj.sourceUrls, 4, 500)
        };
      })
      .filter(Boolean);

    return {
      brand: {
        name: normalizeText(brand.name, "Aha Shine", 80),
        agentName: normalizeText(brand.agentName, "Trợ lý", 80),
        launcherLabel: normalizeText(brand.launcherLabel, "Aha", 60),
        welcomeTitle: normalizeText(brand.welcomeTitle, "Xin chào, mình có thể hỗ trợ gì?", 120),
        welcomeMessage: normalizeText(
          brand.welcomeMessage,
          "Mời bạn chọn nhanh nội dung bên dưới hoặc nhập câu hỏi.",
          600
        ),
        inputPlaceholder: normalizeText(brand.inputPlaceholder, "Nhập tin nhắn...", 100),
        logoDataUrl: normalizeText(brand.logoDataUrl, "", 3 * 1024 * 1024)
      },
      theme: {
        primaryColor: normalizeColor(theme.primaryColor, "#39b54a"),
        launcherBg: normalizeColor(theme.launcherBg, "#ffd33d"),
        textOnPrimary: normalizeColor(theme.textOnPrimary, "#ffffff"),
        panelBg: normalizeColor(theme.panelBg, "#ffffff"),
        botBubbleBg: normalizeColor(theme.botBubbleBg, "#f2f4f7"),
        userBubbleBg: normalizeColor(theme.userBubbleBg, "#fff6d8")
      },
      layout: {
        position: layout.position === "left" ? "left" : "right",
        bottom: normalizeNumber(layout.bottom, 20, 0, 120),
        side: normalizeNumber(layout.side, 20, 0, 120),
        zIndex: normalizeNumber(layout.zIndex, 2147482000, 1000, 2147483000)
      },
      behavior: {
        showWelcomeOnOpen: normalizeBool(behavior.showWelcomeOnOpen, true),
        startOpen: normalizeBool(behavior.startOpen, false),
        maxMessages: normalizeNumber(behavior.maxMessages, 120, 20, 500),
        typingDelayMs: normalizeNumber(behavior.typingDelayMs, 300, 0, 2000)
      },
      quickStartButtons: normalizeArray(source.quickStartButtons, 12, 80),
      knowledgeBase,
      documents,
      fallbackAnswer: normalizeText(
        source.fallbackAnswer,
        "Mình đã ghi nhận câu hỏi của bạn. Bạn có thể để lại số điện thoại để nhân viên hỗ trợ thêm nhé.",
        2000
      )
    };
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

  const MATCH_STOP_WORDS = new Set([
    "la",
    "va",
    "co",
    "cho",
    "toi",
    "ban",
    "anh",
    "chi",
    "minh",
    "duoc",
    "khong",
    "cua",
    "the",
    "nay",
    "neu",
    "thi",
    "voi",
    "ve",
    "mot",
    "nhieu",
    "trong",
    "ngoai",
    "nhung",
    "cac",
    "tu",
    "den",
    "tai",
    "khi",
    "sao",
    "theo",
    "gi",
    "nao",
    "help",
    "info",
    "for",
    "the",
    "and",
    "with",
    "from",
    "you"
  ]);

  function tokenizeText(text) {
    const folded = foldText(text);
    if (!folded) {
      return [];
    }
    return Array.from(
      new Set(
        folded
      .split(/[^a-z0-9]+/g)
      .map((item) => item.trim())
          .filter((item) => item.length >= 2 && !MATCH_STOP_WORDS.has(item))
      )
    ).slice(0, 28);
  }

  function countOccurrences(text, term) {
    if (!text || !term) {
      return 0;
    }
    let count = 0;
    let fromIndex = 0;
    while (fromIndex < text.length) {
      const index = text.indexOf(term, fromIndex);
      if (index < 0) {
        break;
      }
      count += 1;
      fromIndex = index + term.length;
    }
    return count;
  }

  function getQueryBigrams(tokens) {
    const rows = Array.isArray(tokens) ? tokens : [];
    const out = [];
    for (let i = 0; i < rows.length - 1; i += 1) {
      const phrase = `${rows[i]} ${rows[i + 1]}`.trim();
      if (phrase.length >= 5) {
        out.push(phrase);
      }
    }
    return out.slice(0, 12);
  }

  function scoreKeywordMatch(keywordFolded, queryFolded, queryTokensSet) {
    if (!keywordFolded) {
      return 0;
    }
    const keywordTokens = tokenizeText(keywordFolded);
    if (keywordTokens.length === 0) {
      return 0;
    }
    if (queryFolded === keywordFolded) {
      return 15 + Math.min(3, keywordTokens.length);
    }
    if (queryFolded.includes(keywordFolded) && keywordFolded.length >= 3) {
      return 11 + Math.min(4, keywordTokens.length);
    }
    let hits = 0;
    for (let i = 0; i < keywordTokens.length; i += 1) {
      if (queryTokensSet.has(keywordTokens[i])) {
        hits += 1;
      }
    }
    if (hits === 0) {
      return 0;
    }
    if (keywordTokens.length === 1) {
      const token = keywordTokens[0];
      if (token.length <= 2) {
        return 0;
      }
      return hits > 0 ? 3.2 : 0;
    }
    const coverage = hits / keywordTokens.length;
    if (coverage >= 1) {
      return 8 + hits * 0.9;
    }
    if (coverage >= 0.66) {
      return 4.8 + coverage * 2;
    }
    return 0;
  }

  function findKnowledgeMatch(config, userText, context) {
    const rows = Array.isArray(config.knowledgeBase) ? config.knowledgeBase : [];
    if (rows.length === 0) {
      return null;
    }
    const queryFolded = foldText(userText);
    const queryTokens = tokenizeText(userText);
    if (!queryFolded || queryTokens.length === 0) {
      return null;
    }
    const queryTokensSet = new Set(queryTokens);
    const contextTitleFolded = foldText(context && context.title ? context.title : "");
    let best = null;
    let secondScore = 0;
    for (let i = 0; i < rows.length; i += 1) {
      const row = rows[i];
      const keywords = Array.isArray(row.keywords) ? row.keywords : [];
      let bestKeywordScore = 0;
      let matchedKeyword = "";
      for (let j = 0; j < keywords.length; j += 1) {
        const keywordFolded = foldText(keywords[j]);
        const score = scoreKeywordMatch(keywordFolded, queryFolded, queryTokensSet);
        if (score > bestKeywordScore) {
          bestKeywordScore = score;
          matchedKeyword = keywordFolded;
        }
      }
      if (bestKeywordScore <= 0) {
        continue;
      }
      if (matchedKeyword && contextTitleFolded && contextTitleFolded.includes(matchedKeyword)) {
        bestKeywordScore += 0.8;
      }
      if (bestKeywordScore > (best ? best.score : 0)) {
        secondScore = best ? best.score : secondScore;
        best = {
          row,
          score: bestKeywordScore
        };
      } else if (bestKeywordScore > secondScore) {
        secondScore = bestKeywordScore;
      }
    }
    if (!best) {
      return null;
    }
    const confidenceGap = best.score - secondScore;
    if (best.score < 4.4) {
      return null;
    }
    if (confidenceGap < 0.55 && best.score < 8.8) {
      return null;
    }
    return best.row;
  }

  function buildDocumentSnippet(text, tokens) {
    const source = normalizeRichTextSource(text);
    if (!source) {
      return "";
    }
    const clean = source.replace(/\s+/g, " ").trim();
    if (clean.length <= 320) {
      return clean;
    }
    const folded = foldText(clean);
    let position = -1;
    for (let i = 0; i < tokens.length; i += 1) {
      const token = foldText(tokens[i]);
      if (!token || token.length < 3) {
        continue;
      }
      const found = folded.indexOf(token);
      if (found >= 0) {
        position = found;
        break;
      }
    }
    if (position < 0) {
      return `${clean.slice(0, 320)}...`;
    }
    const start = Math.max(0, position - 110);
    const end = Math.min(clean.length, start + 320);
    const chunk = clean.slice(start, end).trim();
    const prefix = start > 0 ? "..." : "";
    const suffix = end < clean.length ? "..." : "";
    return `${prefix}${chunk}${suffix}`;
  }

  function findDocumentMatch(config, userText, context) {
    const docs = Array.isArray(config.documents) ? config.documents : [];
    if (docs.length === 0) {
      return null;
    }
    const foldedUser = foldText(userText);
    const tokens = tokenizeText(userText);
    if (!foldedUser || tokens.length === 0) {
      return null;
    }
    const queryTokenSet = new Set(tokens);
    const bigrams = getQueryBigrams(tokens);
    const contextUrlFolded = foldText(context && context.url ? context.url : "");
    const contextTitleFolded = foldText(context && context.title ? context.title : "");
    let bestScore = 0;
    let bestDoc = null;
    for (let i = 0; i < docs.length; i += 1) {
      const doc = docs[i];
      const titleFolded = foldText(doc.title || "");
      const textFolded = foldText(doc.text || "");
      const tagsFolded = foldText(Array.isArray(doc.tags) ? doc.tags.join(" ") : "");
      const sourceUrls = Array.isArray(doc.sourceUrls) ? doc.sourceUrls : [];
      const sourceUrlFolded = sourceUrls.map((item) => foldText(item)).filter(Boolean);
      let score = 0;
      if (foldedUser && titleFolded.includes(foldedUser)) {
        score += 6.8;
      }
      if (foldedUser && tagsFolded.includes(foldedUser)) {
        score += 5.5;
      }
      if (foldedUser && textFolded.includes(foldedUser)) {
        score += 3.2;
      }
      let tokenHits = 0;
      for (let j = 0; j < tokens.length; j += 1) {
        const token = tokens[j];
        if (!token || token.length < 3 || !queryTokenSet.has(token)) {
          continue;
        }
        const inTitle = titleFolded.includes(token);
        const inTags = tagsFolded.includes(token);
        const occursText = countOccurrences(textFolded, token);
        if (inTitle || inTags || occursText > 0) {
          tokenHits += 1;
        }
        if (inTitle) {
          score += 2.6;
        }
        if (inTags) {
          score += 2.2;
        }
        if (occursText > 0) {
          score += 0.9 + Math.min(occursText, 3) * 0.25;
        }
      }
      if (tokens.length > 0) {
        score += tokenHits / tokens.length;
      }
      for (let j = 0; j < bigrams.length; j += 1) {
        const phrase = bigrams[j];
        if (!phrase) {
          continue;
        }
        if (titleFolded.includes(phrase)) {
          score += 1.6;
        }
        if (tagsFolded.includes(phrase)) {
          score += 1.2;
        }
        if (textFolded.includes(phrase)) {
          score += 0.7;
        }
      }
      if (contextUrlFolded && sourceUrlFolded.some((item) => item.includes(contextUrlFolded) || contextUrlFolded.includes(item))) {
        score += 2.8;
      }
      if (contextTitleFolded && contextTitleFolded.length >= 6) {
        if (titleFolded.includes(contextTitleFolded.slice(0, 60))) {
          score += 0.9;
        }
      }
      if (score > bestScore) {
        bestScore = score;
        bestDoc = doc;
      }
    }
    if (!bestDoc || bestScore < 3.8) {
      return null;
    }
    const snippet = buildDocumentSnippet(bestDoc.text || "", tokens);
    const sourceUrl = Array.isArray(bestDoc.sourceUrls) && bestDoc.sourceUrls.length > 0
      ? bestDoc.sourceUrls[0]
      : "";
    const answerLines = [];
    answerLines.push(`**${normalizeText(bestDoc.title, "Thông tin liên quan", 180)}**`);
    if (snippet) {
      answerLines.push(snippet);
    }
    if (sourceUrl) {
      answerLines.push(`**Tham khảo thêm:** [${sourceUrl}](${sourceUrl})`);
    }
    return {
      answer: answerLines.join("\n\n"),
      quickReplies: normalizeArray(bestDoc.tags, 3, 50)
    };
  }

  function createSvgIcon() {
    const wrapper = document.createElement("div");
    wrapper.innerHTML =
      '<svg viewBox="0 0 24 24" width="24" height="24" aria-hidden="true" focusable="false"><path fill="currentColor" d="M4 12c0-4.42 3.58-8 8-8h8v11a6 6 0 0 1-6 6H4v-9z"></path><circle cx="9" cy="12" r="1.2" fill="#ffffff"></circle><circle cx="13" cy="12" r="1.2" fill="#ffffff"></circle><path d="M8.3 15.2c1 .9 2.4 1.4 3.7 1.4 1.3 0 2.7-.5 3.7-1.4" stroke="#ffffff" stroke-width="1.3" fill="none" stroke-linecap="round"></path></svg>';
    return wrapper.firstChild;
  }

  function decodeHtmlEntities(text) {
    const value = (text || "").toString();
    if (!value) {
      return "";
    }
    const textarea = document.createElement("textarea");
    textarea.innerHTML = value;
    return textarea.value;
  }

  function stripHtmlTags(text) {
    return decodeHtmlEntities((text || "").toString().replace(/<[^>]+>/g, " "))
      .replace(/\s+/g, " ")
      .trim();
  }

  function normalizeRichTextSource(text) {
    let output = (text || "").toString();
    if (!output) {
      return "";
    }
    output = output.replace(/\r\n?/g, "\n");
    output = output.replace(/<br\s*\/?>/gi, "\n");
    output = output.replace(/<\/p>/gi, "\n");
    output = output.replace(/<p[^>]*>/gi, "");
    output = output.replace(/<a\b[^>]*href\s*=\s*["']([^"']+)["'][^>]*>([\s\S]*?)<\/a>/gi, function (_, href, label) {
      const cleanLabel = stripHtmlTags(label) || href;
      return `[${cleanLabel}](${href})`;
    });
    output = output.replace(/<(strong|b)[^>]*>([\s\S]*?)<\/\1>/gi, function (_, __, inner) {
      const clean = stripHtmlTags(inner);
      return clean ? `**${clean}**` : "";
    });
    output = output.replace(/<(em|i)[^>]*>([\s\S]*?)<\/\1>/gi, function (_, __, inner) {
      const clean = stripHtmlTags(inner);
      return clean ? `*${clean}*` : "";
    });
    output = decodeHtmlEntities(output.replace(/<[^>]+>/g, " "));
    output = output.replace(/[ \t]+\n/g, "\n");
    output = output.replace(/\n{3,}/g, "\n\n");
    return output.trim();
  }

  function normalizeHref(href) {
    try {
      const url = new URL((href || "").toString(), window.location.href);
      if (!/^https?:$/i.test(url.protocol)) {
        return "";
      }
      return url.href;
    } catch (_) {
      return "";
    }
  }

  function appendFormattedText(parent, text) {
    const source = (text || "").toString();
    if (!source) {
      return;
    }
    const emphasisRegex = /(\*\*([^*]+)\*\*|\*([^*\n]+)\*)/g;
    let lastIndex = 0;
    let match = emphasisRegex.exec(source);
    while (match) {
      if (match.index > lastIndex) {
        parent.appendChild(document.createTextNode(source.slice(lastIndex, match.index)));
      }
      if (match[2]) {
        const strong = document.createElement("strong");
        strong.textContent = match[2];
        parent.appendChild(strong);
      } else if (match[3]) {
        const em = document.createElement("em");
        em.textContent = match[3];
        parent.appendChild(em);
      }
      lastIndex = match.index + match[0].length;
      match = emphasisRegex.exec(source);
    }
    if (lastIndex < source.length) {
      parent.appendChild(document.createTextNode(source.slice(lastIndex)));
    }
  }

  function appendRichTextLine(parent, line) {
    const source = (line || "").toString();
    if (!source) {
      return;
    }
    const linkRegex = /\[([^\]]+)\]\((https?:\/\/[^\s)]+)\)|(https?:\/\/[^\s<]+)/g;
    let lastIndex = 0;
    let match = linkRegex.exec(source);
    while (match) {
      if (match.index > lastIndex) {
        appendFormattedText(parent, source.slice(lastIndex, match.index));
      }
      const href = normalizeHref(match[2] || match[3] || "");
      const label = (match[1] || href || "").trim();
      if (href) {
        const link = document.createElement("a");
        const sameOrigin = href.indexOf(`${window.location.origin}/`) === 0 || href === window.location.origin;
        link.href = href;
        link.textContent = label || href;
        link.className = "aha-link";
        link.target = sameOrigin ? "_self" : "_blank";
        link.rel = "noopener noreferrer";
        parent.appendChild(link);
      } else {
        appendFormattedText(parent, match[0]);
      }
      lastIndex = match.index + match[0].length;
      match = linkRegex.exec(source);
    }
    if (lastIndex < source.length) {
      appendFormattedText(parent, source.slice(lastIndex));
    }
  }

  function createRichTextFragment(text) {
    const fragment = document.createDocumentFragment();
    const normalized = normalizeRichTextSource(text);
    if (!normalized) {
      return fragment;
    }
    const lines = normalized.split("\n");
    for (let i = 0; i < lines.length; i += 1) {
      appendRichTextLine(fragment, lines[i]);
      if (i < lines.length - 1) {
        fragment.appendChild(document.createElement("br"));
      }
    }
    return fragment;
  }

  function initWidget(config, options) {
    const host = document.createElement("div");
    host.setAttribute("data-aha-standalone-widget", "1");
    host.style.position = "fixed";
    host.style.inset = "0";
    host.style.pointerEvents = "none";
    host.style.zIndex = String(config.layout.zIndex);
    document.body.appendChild(host);

    const shadow = host.attachShadow({ mode: "open" });

    const style = document.createElement("style");
    style.textContent = `
      @import url("https://fonts.googleapis.com/css2?family=Be+Vietnam+Pro:wght@300;400;500;600;700;800&display=swap");
      :host {
        all: initial;
      }
      * {
        box-sizing: border-box;
      }
      .aha-root {
        position: fixed;
        bottom: ${config.layout.bottom}px;
        ${config.layout.position === "left" ? `left: ${config.layout.side}px;` : `right: ${config.layout.side}px;`}
        z-index: ${config.layout.zIndex};
        display: grid;
        gap: 10px;
        font-family: "Be Vietnam Pro", "Segoe UI", Arial, sans-serif;
        pointer-events: none;
      }
      .aha-root.open {
        gap: 0;
      }
      .aha-root.open .aha-launcher,
      .aha-root.open .aha-launcher-label {
        display: none;
      }
      .aha-launcher {
        pointer-events: auto;
        justify-self: ${config.layout.position === "left" ? "start" : "end"};
        width: 66px;
        height: 66px;
        border: 0;
        border-radius: 999px;
        background: ${config.theme.launcherBg};
        color: #1a1f2b;
        display: grid;
        place-items: center;
        box-shadow: 0 10px 26px rgba(20, 24, 31, 0.22);
        cursor: pointer;
      }
      .aha-launcher-inner {
        width: 54px;
        height: 54px;
        border-radius: 50%;
        border: 2px solid rgba(255, 255, 255, 0.95);
        overflow: hidden;
        background: #ffffff;
        display: grid;
        place-items: center;
      }
      .aha-launcher-inner img {
        width: 100%;
        height: 100%;
        object-fit: cover;
        display: block;
      }
      .aha-launcher-label {
        pointer-events: none;
        margin-top: 6px;
        text-align: center;
        font-size: 13px;
        font-weight: 700;
        text-shadow: 0 1px 2px rgba(0, 0, 0, 0.35);
        color: #ffffff;
      }
      .aha-panel {
        width: min(366px, calc(100vw - 24px));
        height: min(520px, calc(var(--aha-visible-height, 100dvh) - 112px));
        max-height: calc(var(--aha-visible-height, 100dvh) - 112px);
        border-radius: 16px;
        border: 1px solid rgba(16, 22, 35, 0.1);
        overflow: hidden;
        background: ${config.theme.panelBg};
        box-shadow: 0 24px 48px rgba(20, 24, 31, 0.24);
        display: none;
        flex-direction: column;
        pointer-events: auto;
      }
      .aha-root.open .aha-panel {
        display: flex;
      }
      .aha-header {
        background: ${config.theme.primaryColor};
        color: ${config.theme.textOnPrimary};
        display: flex;
        align-items: center;
        justify-content: space-between;
        gap: 10px;
        padding: 12px 14px;
      }
      .aha-header-title {
        display: flex;
        align-items: center;
        gap: 10px;
        min-width: 0;
      }
      .aha-header-logo {
        width: 34px;
        height: 34px;
        border-radius: 50%;
        border: 2px solid rgba(255, 255, 255, 0.92);
        overflow: hidden;
        background: #fff;
        flex: 0 0 auto;
        display: grid;
        place-items: center;
      }
      .aha-header-logo img {
        width: 100%;
        height: 100%;
        object-fit: cover;
        display: block;
      }
      .aha-header-text {
        min-width: 0;
      }
      .aha-header-text strong {
        display: block;
        font-size: 15px;
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
      }
      .aha-header-text span {
        display: block;
        font-size: 12px;
        opacity: 0.88;
      }
      .aha-close {
        border: 0;
        background: transparent;
        color: inherit;
        font-size: 24px;
        width: 34px;
        height: 34px;
        border-radius: 10px;
        cursor: pointer;
      }
      .aha-close:hover {
        background: rgba(255, 255, 255, 0.16);
      }
      .aha-messages {
        flex: 1;
        overflow: auto;
        padding: 12px;
        background: #fafbff;
        display: grid;
        align-content: start;
        gap: 10px;
      }
      .aha-turn {
        display: grid;
        gap: 8px;
      }
      .aha-turn.user {
        justify-items: end;
      }
      .aha-turn.bot {
        justify-items: start;
      }
      .aha-msg {
        max-width: 82%;
        border-radius: 12px;
        padding: 9px 11px;
        line-height: 1.45;
        font-size: 14px;
        color: #171d28;
        box-shadow: 0 1px 1px rgba(18, 24, 40, 0.06);
        white-space: normal;
        word-break: break-word;
      }
      .aha-msg.bot {
        background: ${config.theme.botBubbleBg};
      }
      .aha-msg.user {
        background: ${config.theme.userBubbleBg};
      }
      .aha-msg strong {
        font-weight: 800;
      }
      .aha-msg em {
        font-style: italic;
      }
      .aha-msg a,
      .aha-link {
        color: #1565c0;
        text-decoration: underline;
        font-weight: 600;
      }
      .aha-msg.typing {
        width: 56px;
        text-align: center;
        letter-spacing: 2px;
        font-weight: 700;
      }
      .aha-quick {
        display: flex;
        flex-wrap: wrap;
        gap: 8px;
      }
      .aha-quick-btn {
        border: 1px solid rgba(18, 24, 40, 0.18);
        background: #ffffff;
        border-radius: 999px;
        padding: 7px 11px;
        font-size: 13px;
        cursor: pointer;
      }
      .aha-quick-btn:hover {
        background: #f2f5fb;
      }
      .aha-form {
        border-top: 1px solid #e3e8f1;
        padding: 10px;
        display: grid;
        grid-template-columns: 1fr auto;
        gap: 8px;
        background: #fff;
      }
      .aha-input {
        width: 100%;
        border: 1px solid #d2dae7;
        border-radius: 999px;
        font: inherit;
        font-size: 14px;
        padding: 10px 14px;
        color: #171d28;
        outline: none;
      }
      .aha-input:focus {
        border-color: ${config.theme.primaryColor};
      }
      .aha-send {
        border: 0;
        border-radius: 999px;
        background: ${config.theme.primaryColor};
        color: ${config.theme.textOnPrimary};
        min-width: 44px;
        padding: 0 14px;
        cursor: pointer;
      }
      .aha-send:disabled {
        opacity: 0.5;
        cursor: default;
      }
      @media (max-width: 768px) {
        .aha-root {
          left: 12px !important;
          right: 12px !important;
          bottom: 12px;
        }
        .aha-launcher {
          justify-self: end;
        }
        .aha-panel {
          width: 100%;
          height: min(520px, calc(var(--aha-visible-height, 100dvh) - 132px));
          max-height: calc(var(--aha-visible-height, 100dvh) - 132px);
          border-radius: 14px;
        }
      }
    `;

    const root = document.createElement("div");
    root.className = "aha-root";

    function updateVisibleHeightVar() {
      const viewport = window.visualViewport;
      const nextHeight =
        viewport && Number.isFinite(viewport.height) ? viewport.height : window.innerHeight;
      if (!Number.isFinite(nextHeight) || nextHeight <= 0) {
        return;
      }
      root.style.setProperty("--aha-visible-height", `${Math.round(nextHeight)}px`);
    }

    updateVisibleHeightVar();
    window.addEventListener("resize", updateVisibleHeightVar, { passive: true });
    if (window.visualViewport) {
      window.visualViewport.addEventListener("resize", updateVisibleHeightVar, { passive: true });
      window.visualViewport.addEventListener("scroll", updateVisibleHeightVar, { passive: true });
    }

    const panel = document.createElement("section");
    panel.className = "aha-panel";
    panel.setAttribute("role", "dialog");
    panel.setAttribute("aria-label", config.brand.name);

    const header = document.createElement("header");
    header.className = "aha-header";

    const headerTitle = document.createElement("div");
    headerTitle.className = "aha-header-title";

    const headerLogo = document.createElement("div");
    headerLogo.className = "aha-header-logo";
    if (config.brand.logoDataUrl) {
      const img = document.createElement("img");
      img.src = config.brand.logoDataUrl;
      img.alt = config.brand.name;
      headerLogo.appendChild(img);
    } else {
      headerLogo.appendChild(createSvgIcon());
    }

    const headerText = document.createElement("div");
    headerText.className = "aha-header-text";
    const headerStrong = document.createElement("strong");
    headerStrong.textContent = config.brand.agentName;
    const headerSpan = document.createElement("span");
    headerSpan.textContent = config.brand.name;
    headerText.appendChild(headerStrong);
    headerText.appendChild(headerSpan);

    headerTitle.appendChild(headerLogo);
    headerTitle.appendChild(headerText);

    const closeButton = document.createElement("button");
    closeButton.className = "aha-close";
    closeButton.type = "button";
    closeButton.setAttribute("aria-label", "Đóng trò chuyện");
    closeButton.textContent = "×";
    header.appendChild(headerTitle);
    header.appendChild(closeButton);

    const messages = document.createElement("div");
    messages.className = "aha-messages";

    const form = document.createElement("form");
    form.className = "aha-form";
    form.innerHTML = `
      <input class="aha-input" type="text" maxlength="1000" autocomplete="off" />
      <button class="aha-send" type="submit">Gửi</button>
    `;

    const input = form.querySelector(".aha-input");
    const sendButton = form.querySelector(".aha-send");
    input.placeholder = config.brand.inputPlaceholder;

    panel.appendChild(header);
    panel.appendChild(messages);
    panel.appendChild(form);

    const launcher = document.createElement("button");
    launcher.className = "aha-launcher";
    launcher.type = "button";
    launcher.setAttribute("aria-label", config.brand.welcomeTitle);

    const launcherInner = document.createElement("div");
    launcherInner.className = "aha-launcher-inner";
    if (config.brand.logoDataUrl) {
      const image = document.createElement("img");
      image.src = config.brand.logoDataUrl;
      image.alt = config.brand.name;
      launcherInner.appendChild(image);
    } else {
      launcherInner.appendChild(createSvgIcon());
    }
    launcher.appendChild(launcherInner);

    const launcherLabel = document.createElement("div");
    launcherLabel.className = "aha-launcher-label";
    launcherLabel.textContent = config.brand.launcherLabel;

    root.appendChild(panel);
    root.appendChild(launcher);
    root.appendChild(launcherLabel);

    shadow.appendChild(style);
    shadow.appendChild(root);

    let greeted = false;
    let activeJobs = 0;
    let chain = Promise.resolve();
    let conversation = [];
    const storageKey = [
      "__AHA_WIDGET_STATE__",
      normalizeText(options.widgetKey || "", "widget", 120),
      normalizeText(options.projectId || "", "default", 80),
      normalizeText(window.location.origin, "origin", 200)
    ].join("::");

    function readStoredState() {
      try {
        const raw = window.sessionStorage.getItem(storageKey);
        if (!raw) {
          return null;
        }
        const parsed = JSON.parse(raw);
        if (!parsed || typeof parsed !== "object") {
          return null;
        }
        return parsed;
      } catch (_) {
        return null;
      }
    }

    function saveStoredState() {
      try {
        const payload = {
          open: root.classList.contains("open"),
          greeted,
          conversation: conversation.slice(-Math.min(config.behavior.maxMessages, 60))
        };
        window.sessionStorage.setItem(storageKey, JSON.stringify(payload));
      } catch (_) {}
    }

    window.addEventListener("pagehide", saveStoredState, { passive: true });
    window.addEventListener("beforeunload", saveStoredState, { passive: true });

    function trimMessagesIfNeeded() {
      const limit = config.behavior.maxMessages;
      while (messages.children.length > limit) {
        messages.removeChild(messages.firstChild);
      }
      while (conversation.length > limit) {
        conversation.shift();
      }
    }

    function scrollToBottom() {
      messages.scrollTop = messages.scrollHeight;
    }

    function addMessage(role, text, quickReplies, opts) {
      const options = opts && typeof opts === "object" ? opts : {};
      const turn = document.createElement("div");
      turn.className = `aha-turn ${role === "user" ? "user" : "bot"}`;
      const msg = document.createElement("div");
      msg.className = `aha-msg ${role === "user" ? "user" : "bot"}`;
      msg.appendChild(createRichTextFragment(text));
      turn.appendChild(msg);
      if (Array.isArray(quickReplies) && quickReplies.length > 0) {
        const quickWrap = document.createElement("div");
        quickWrap.className = "aha-quick";
        quickReplies.forEach((item) => {
          const button = document.createElement("button");
          button.type = "button";
          button.className = "aha-quick-btn";
          button.textContent = item;
          button.addEventListener("click", function () {
            sendUserMessage(item);
          });
          quickWrap.appendChild(button);
        });
        turn.appendChild(quickWrap);
      }
      Array.from(turn.querySelectorAll("a")).forEach(function (link) {
        link.addEventListener("click", function () {
          saveStoredState();
        });
      });
      messages.appendChild(turn);
      if (options.persist !== false) {
        conversation.push({
          role: role === "user" ? "user" : "bot",
          text: normalizeText((text || "").toString(), "", 4000),
          quickReplies: normalizeArray(quickReplies, 8, 80)
        });
      }
      trimMessagesIfNeeded();
      scrollToBottom();
      saveStoredState();
      return turn;
    }

    function addTyping() {
      const turn = document.createElement("div");
      turn.className = "aha-turn bot";
      const msg = document.createElement("div");
      msg.className = "aha-msg bot typing";
      msg.textContent = "...";
      turn.appendChild(msg);
      messages.appendChild(turn);
      trimMessagesIfNeeded();
      scrollToBottom();
      return turn;
    }

    function restoreStoredConversation() {
      const state = readStoredState();
      if (!state) {
        return false;
      }
      const items = Array.isArray(state.conversation) ? state.conversation : [];
      conversation = [];
      messages.innerHTML = "";
      items.forEach(function (item) {
        if (!item || typeof item !== "object") {
          return;
        }
        addMessage(item.role === "user" ? "user" : "bot", item.text || "", item.quickReplies || [], {
          persist: false
        });
      });
      conversation = items
        .map(function (item) {
          if (!item || typeof item !== "object") {
            return null;
          }
          return {
            role: item.role === "user" ? "user" : "bot",
            text: normalizeText((item.text || "").toString(), "", 4000),
            quickReplies: normalizeArray(item.quickReplies, 8, 80)
          };
        })
        .filter(Boolean);
      greeted = Boolean(state.greeted) || conversation.some(function (item) {
        return item.role === "bot";
      });
      if (state.open) {
        root.classList.add("open");
      }
      saveStoredState();
      return conversation.length > 0;
    }

    function showWelcomeIfNeeded() {
      if (greeted || !config.behavior.showWelcomeOnOpen) {
        return;
      }
      greeted = true;
      addMessage("bot", `${config.brand.welcomeTitle}\n${config.brand.welcomeMessage}`, config.quickStartButtons);
    }

    function openWidget() {
      root.classList.add("open");
      showWelcomeIfNeeded();
      saveStoredState();
      window.setTimeout(function () {
        input.focus();
      }, 0);
    }

    function closeWidget() {
      root.classList.remove("open");
      saveStoredState();
    }

    function fallbackReplyFor(message) {
      const context = {
        url: window.location.href,
        title: document.title || ""
      };
      const matched = findKnowledgeMatch(config, message, context);
      if (matched) {
        return {
          answer: matched.answer,
          quickReplies: matched.quickReplies
        };
      }
      const docMatch = findDocumentMatch(config, message, context);
      if (docMatch) {
        return docMatch;
      }
      return {
        answer: config.fallbackAnswer,
        quickReplies: config.quickStartButtons.slice(0, 6)
      };
    }

  async function fetchServerReply(message) {
      const endpoint = typeof options.chatUrl === "string" ? options.chatUrl.trim() : "";
      if (!endpoint) {
        throw new Error("NO_REMOTE_CHAT_ENDPOINT");
      }
      const context = {
        url: window.location.href,
        title: document.title || ""
      };
      const projectId =
        typeof options.projectId === "string" && options.projectId.trim() ? options.projectId.trim() : "";
      const widgetKey =
        typeof options.widgetKey === "string" && options.widgetKey.trim() ? options.widgetKey.trim() : "";
      const response = await fetch(endpoint, {
        method: "POST",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify({ message, context, projectId, widgetKey })
      });
      if (!response.ok) {
        throw new Error(`Lỗi HTTP ${response.status}`);
      }
      const payload = await response.json();
      if (!payload || typeof payload.answer !== "string") {
        throw new Error("Phản hồi chat không hợp lệ");
      }
      return {
        answer: payload.answer,
        quickReplies: Array.isArray(payload.quickReplies) ? payload.quickReplies : []
      };
    }

    async function sendUserMessageInternal(text) {
      const value = normalizeText(text, "", 1000);
      if (!value) {
        return;
      }
      addMessage("user", value);
      activeJobs += 1;
      sendButton.disabled = true;
      const typing = addTyping();
      const minDelay = Math.max(config.behavior.typingDelayMs, 80);
      const startedAt = Date.now();

      let reply;
      try {
        reply = await fetchServerReply(value);
      } catch (_) {
        reply = fallbackReplyFor(value);
      }

      const elapsed = Date.now() - startedAt;
      const waiting = Math.max(0, minDelay - elapsed);
      if (waiting > 0) {
        await new Promise((resolve) => window.setTimeout(resolve, waiting));
      }

      typing.remove();
      addMessage("bot", reply.answer, reply.quickReplies);
      activeJobs = Math.max(0, activeJobs - 1);
      sendButton.disabled = activeJobs > 0;
      input.focus();
    }

    function sendUserMessage(text) {
      chain = chain.then(function () {
        return sendUserMessageInternal(text);
      });
    }

    launcher.addEventListener("click", function () {
      if (root.classList.contains("open")) {
        closeWidget();
      } else {
        openWidget();
      }
    });

    closeButton.addEventListener("click", function () {
      closeWidget();
    });

    form.addEventListener("submit", function (event) {
      event.preventDefault();
      const value = input.value.trim();
      if (!value) {
        return;
      }
      input.value = "";
      sendUserMessage(value);
    });

    input.addEventListener("keydown", function (event) {
      if (event.key === "Escape") {
        closeWidget();
      }
    });

    const restored = restoreStoredConversation();
    if (root.classList.contains("open")) {
      window.setTimeout(function () {
        input.focus();
      }, 0);
    }

    if (!restored && (config.behavior.startOpen || normalizeBool(scriptElement.getAttribute("data-start-open"), false))) {
      openWidget();
    }
  }

  async function bootstrap() {
    const baseUrl = resolveBaseUrl(scriptElement);
    const configUrlAttr = (scriptElement.getAttribute("data-config-url") || "").trim();
    const chatUrlAttr = (scriptElement.getAttribute("data-chat-url") || "").trim();
    const widgetKeyAttr = (scriptElement.getAttribute("data-widget-key") || "").trim();
    const projectIdAttr = (scriptElement.getAttribute("data-project-id") || "").trim();
    const keyQuery = widgetKeyAttr ? `widgetKey=${encodeURIComponent(widgetKeyAttr)}` : "";
    const configUrl = configUrlAttr || `${baseUrl}/widget-data/widget-data.json`;
    const chatUrl = chatUrlAttr
      ? chatUrlAttr
      : "";
    const finalChatUrl = keyQuery && chatUrl.includes("?")
      ? `${chatUrl}&${keyQuery}`
      : keyQuery && chatUrl
        ? `${chatUrl}?${keyQuery}`
        : chatUrl;
    try {
      const response = await fetch(configUrl, {
        method: "GET",
        cache: "no-store"
      });
      if (!response.ok) {
        throw new Error(`Lỗi HTTP ${response.status}`);
      }
      const config = await response.json();
      initWidget(normalizeConfig(config, { widgetKey: widgetKeyAttr }), {
        chatUrl: finalChatUrl,
        projectId: projectIdAttr,
        widgetKey: widgetKeyAttr
      });
    } catch (error) {
      initWidget(normalizeConfig({}, { widgetKey: widgetKeyAttr }), {
        chatUrl: finalChatUrl,
        projectId: projectIdAttr,
        widgetKey: widgetKeyAttr
      });
      if (window.console && typeof window.console.error === "function") {
        window.console.error("[AHA Widget] Không thể tải cấu hình từ xa:", error);
      }
    }
  }

  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", bootstrap, { once: true });
  } else {
    bootstrap();
  }
})();
