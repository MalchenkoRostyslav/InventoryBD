/* Спільна логіка: API, шапка, тости, відображення предметів */
const $ = (s, r = document) => r.querySelector(s);

async function api(url, method = 'GET', body) {
  const r = await fetch(url, {
    method,
    headers: { 'Content-Type': 'application/json', 'X-Player-Id': localStorage.getItem('pid') || '0' },
    body: body ? JSON.stringify(body) : undefined
  });
  const d = await r.json().catch(() => ({}));
  if (r.status === 401) { localStorage.removeItem('pid'); location.href = '/index.html'; }
  if (!r.ok) throw new Error(d.error || 'Помилка сервера (' + r.status + ')');
  return d;
}

function toast(msg, bad = false) {
  let box = $('#toasts');
  if (!box) { box = document.createElement('div'); box.id = 'toasts'; document.body.append(box); }
  const t = document.createElement('div');
  t.className = 'toast' + (bad ? ' bad' : '');
  t.textContent = msg;
  box.append(t);
  setTimeout(() => t.remove(), 3500);
}

const slotEmoji = n => {
  n = (n || '').toLowerCase();
  if (/гол|шолом|head|helm/.test(n)) return '🪖';
  if (/тіл|груд|body|chest|брон/.test(n)) return '🛡️';
  if (/зброя|рук|меч|weapon|hand/.test(n)) return '⚔️';
  if (/ног|чобіт|boot|leg|feet/.test(n)) return '🥾';
  return '🎒';
};

/** Іконка: IconPath із wwwroot, а якщо файлу немає — емодзі за типом слота. */
function iconNode(iconPath, slotName) {
  const wrap = document.createElement('span');
  wrap.className = 'ico';
  const fallback = () => { wrap.textContent = slotEmoji(slotName); };
  if (iconPath) {
    const img = document.createElement('img');
    img.src = iconPath.startsWith('/') || iconPath.startsWith('http') ? iconPath : '/' + iconPath;
    img.alt = ''; img.onerror = () => { img.remove(); fallback(); };
    wrap.append(img);
  } else fallback();
  return wrap;
}

function renderCurrency(c) {
  return `<span class="chip" title="Залізо">🔩 <b>${c.iron}</b></span>` +
         `<span class="chip" title="Дерево">🪵 <b>${c.wood}</b></span>` +
         `<span class="chip" title="Тріски">🪨 <b>${c.scrap}</b></span>`;
}

/** Будує шапку і повертає профіль гравця. */
async function initPage(active) {
  if (!localStorage.getItem('pid')) { location.href = '/index.html'; return; }
  const me = await api('/api/player/me');
  const links = [['hero.html', 'Герой'], ['shop.html', 'Крамниця'], ['dungeons.html', 'Підземелля']];
  const el = document.createElement('header');
  el.className = 'top';
  el.innerHTML = `<a class="brand" href="hero.html">Залізний Рубіж</a>
    <nav class="nav">${links.map(([h, t]) => `<a href="${h}" class="${h === active ? 'on' : ''}">${t}</a>`).join('')}</nav>
    <div class="who"><span id="wealth">${renderCurrency(me.currency)}</span>
      <span class="chip">⚔ <b id="gs">${me.gearScore}</b></span>
      <span class="chip">${me.nickname}</span>
      <button class="btn-link" id="logout">Вийти</button></div>`;
  document.body.prepend(el);
  $('#logout').onclick = () => { localStorage.removeItem('pid'); location.href = '/index.html'; };
  return me;
}
const setWealth = c => { $('#wealth').innerHTML = renderCurrency(c); };
const setGear = g => { $('#gs').textContent = g; };

/** DOM-елемент предмета інвентарю (перетягуваний). */
function itemEl(it) {
  const d = document.createElement('div');
  d.className = 'item'; d.draggable = true; d.dataset.id = it.inventoryId;
  d.style.setProperty('--rc', it.colorHex);
  d.title = `${it.itemName} (${it.rarityName})\nСила: ${it.effectivePower}`;
  d.append(iconNode(it.iconPath, it.slotName));
  const p = document.createElement('span'); p.className = 'pw'; p.textContent = it.effectivePower; d.append(p);
  return d;
}
