'use strict';

let travelers = [];
let senders = [];

const travelerForm = document.getElementById('travelerForm');
const senderForm = document.getElementById('senderForm');
const matchList = document.getElementById('matchList');
const noMatches = document.getElementById('noMatches');

// Dropdown elements
const routesBtn = document.getElementById('routesBtn');
const ordersBtn = document.getElementById('ordersBtn');
const routesDropdown = document.getElementById('routesDropdown');
const ordersDropdown = document.getElementById('ordersDropdown');
const routesList = document.getElementById('routesList');
const ordersList = document.getElementById('ordersList');

// Фильтры для поиска
let routesFilter = { from: '', to: '' };
let ordersFilter = { from: '', to: '' };

async function loadData() {
  try {
    const apiBase = 'http://localhost:5000';
    const [travelersRes, sendersRes] = await Promise.all([
      fetch(`${apiBase}/api/travelers`).then(r => r.json()),
      fetch(`${apiBase}/api/senders`).then(r => r.json())
    ]);
    travelers = travelersRes;
    senders = sendersRes;
    findMatches();
    updateRoutesList();
    updateOrdersList();
  } catch (err) {
    console.error('Ошибка загрузки данных с сервера:', err);
    noMatches.textContent = 'Не удалось подключиться к серверу. Проверь, запущен ли C# API на http://localhost:5115';
  }
}

travelerForm.addEventListener('submit', async (e) => {
  e.preventDefault();
  const data = new FormData(travelerForm);
  const from = data.get('from').trim();
  const to = data.get('to').trim();
  const weight = parseFloat(data.get('weight'));
  const reward = parseInt(data.get('reward'));

  if (!from || !to || isNaN(weight) || isNaN(reward) || weight <= 0 || reward < 0) {
    alert('❌ Пожалуйста, заполните все поля корректно.');
    return;
  }

  const newTraveler = { from, to, weight, reward };

  try {
    const apiBase = 'http://localhost:5000';
    const response = await fetch(`${apiBase}/api/travelers`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(newTraveler)
    });

    if (response.ok) {
      const saved = await response.json();
      travelers.push(saved);
      travelerForm.reset();
      findMatches();
      updateRoutesList();
      alert(`✅ Маршрут добавлен: ${from} → ${to}`);
    } else {
      alert('❌ Ошибка при сохранении маршрута.');
    }
  } catch (err) {
    alert('⚠️ Не удалось подключиться к серверу. Убедитесь, что C# API запущен.');
    console.error(err);
  }
});

senderForm.addEventListener('submit', async (e) => {
  e.preventDefault();
  const data = new FormData(senderForm);
  const from = data.get('from').trim();
  const to = data.get('to').trim();
  const weight = parseFloat(data.get('weight'));
  const desc = (data.get('description') || '').toString().trim() || 'Без описания';

  if (!from || !to || isNaN(weight) || weight <= 0) {
    alert('❌ Пожалуйста, заполните все поля корректно.');
    return;
  }

  const newSender = { from, to, weight, description: desc };

  try {
    const apiBase = 'http://localhost:5000';
    const response = await fetch(`${apiBase}/api/senders`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(newSender)
    });

    if (response.ok) {
      const saved = await response.json();
      senders.push(saved);
      senderForm.reset();
      findMatches();
      updateOrdersList();
      alert(`✅ Запрос на доставку добавлен: ${from} → ${to}`);
    } else {
      alert('❌ Ошибка при сохранении запроса.');
    }
  } catch (err) {
    alert('⚠️ Не удалось подключиться к серверу. Убедитесь, что C# API запущен.');
    console.error(err);
  }
});

function findMatches() {
  matchList.innerHTML = '';
  let hasMatches = false;

  senders.forEach(sender => {
    travelers.forEach(traveler => {
      const fromMatch = sender.from.toLowerCase() === traveler.from.toLowerCase();
      const toMatch = sender.to.toLowerCase() === traveler.toLowerCase();
      const weightOk = sender.weight <= traveler.weight;

      if (fromMatch && toMatch && weightOk) {
        const li = document.createElement('li');
        li.innerHTML = `
          <strong>📦 Отправка:</strong> ${sender.from} → ${sender.to} (${sender.weight} кг)<br>
          <strong>💸 Вознаграждение:</strong> ${traveler.reward} ₽<br>
          <small>💬 ${sender.description}</small>
        `;
        matchList.appendChild(li);
        hasMatches = true;
      }
    });
  });

  noMatches.style.display = hasMatches ? 'none' : 'block';
}

// Dropdown functionality
function toggleDropdown(button, dropdown) {
  const isActive = button.classList.contains('active');
  
  // Close all dropdowns
  document.querySelectorAll('.dropdown-btn').forEach(btn => {
    btn.classList.remove('active');
  });
  document.querySelectorAll('.dropdown-content').forEach(content => {
    content.classList.remove('show');
  });
  
  // Toggle current dropdown
  if (!isActive) {
    button.classList.add('active');
    dropdown.classList.add('show');
  }
}

// Event listeners for dropdown buttons
routesBtn.addEventListener('click', () => {
  toggleDropdown(routesBtn, routesDropdown);
  // При открытии сразу обновлять список маршрутов
  updateRoutesList();
});

ordersBtn.addEventListener('click', () => {
  toggleDropdown(ordersBtn, ordersDropdown);
  // При открытии сразу обновлять список заказов
  updateOrdersList();
});

// Close dropdowns when clicking outside
document.addEventListener('click', (e) => {
  if (!e.target.closest('.dropdown')) {
    document.querySelectorAll('.dropdown-btn').forEach(btn => {
      btn.classList.remove('active');
    });
    document.querySelectorAll('.dropdown-content').forEach(content => {
      content.classList.remove('show');
    });
  }
});

// --- Функции для фильтрации и сортировки ---

function filterAndSort(items, filter) {
  // Убираем пробелы до и после введённых символов
  const from = (filter.from || '').trim().toLowerCase();
  const to = (filter.to || '').trim().toLowerCase();
  return items
    .filter(item =>
      (!from || item.from.toLowerCase().includes(from)) &&
      (!to || item.to.toLowerCase().includes(to))
    )
    .sort((a, b) => {
      const aStr = `${a.from.toLowerCase()} ${a.to.toLowerCase()}`;
      const bStr = `${b.from.toLowerCase()} ${b.to.toLowerCase()}`;
      return aStr.localeCompare(bStr);
    });
}

// --- Обновление списков с учетом фильтра ---

function updateRoutesList() {
  routesList.innerHTML = '';

  const filtered = filterAndSort(travelers, routesFilter);

  if (filtered.length === 0) {
    routesList.innerHTML = '<li class="empty-message">Пока нет добавленных маршрутов</li>';
    return;
  }

  filtered.forEach(traveler => {
    const li = document.createElement('li');
    li.className = 'route-item';
    li.innerHTML = `
      <strong>${traveler.from} → ${traveler.to}</strong><br>
      <small>Макс. вес: ${traveler.weight} кг | Вознаграждение: ${traveler.reward} ₽</small>
    `;
    routesList.appendChild(li);
  });
}

function updateOrdersList() {
  ordersList.innerHTML = '';

  const filtered = filterAndSort(senders, ordersFilter);

  if (filtered.length === 0) {
    ordersList.innerHTML = '<li class="empty-message">Пока нет добавленных заказов</li>';
    return;
  }

  filtered.forEach(sender => {
    const li = document.createElement('li');
    li.className = 'order-item';
    li.innerHTML = `
      <strong>${sender.from} → ${sender.to}</strong><br>
      <small>Вес: ${sender.weight} кг | ${sender.description}</small>
    `;
    ordersList.appendChild(li);
  });
}

// --- Обработчики для фильтров поиска ---

function setupDropdownFilters() {
  // Для маршрутов
  const routesFromInput = document.getElementById('routesFilterFrom');
  const routesToInput = document.getElementById('routesFilterTo');
  routesFromInput.addEventListener('input', e => {
    routesFilter.from = e.target.value;
    updateRoutesList();
  });
  routesFromInput.addEventListener('keyup', e => {
    routesFilter.from = e.target.value;
    updateRoutesList();
  });
  routesToInput.addEventListener('input', e => {
    routesFilter.to = e.target.value;
    updateRoutesList();
  });
  routesToInput.addEventListener('keyup', e => {
    routesFilter.to = e.target.value;
    updateRoutesList();
  });

  // Для заказов
  const ordersFromInput = document.getElementById('ordersFilterFrom');
  const ordersToInput = document.getElementById('ordersFilterTo');
  ordersFromInput.addEventListener('input', e => {
    ordersFilter.from = e.target.value;
    updateOrdersList();
  });
  ordersFromInput.addEventListener('keyup', e => {
    ordersFilter.from = e.target.value;
    updateOrdersList();
  });
  ordersToInput.addEventListener('input', e => {
    ordersFilter.to = e.target.value;
    updateOrdersList();
  });
  ordersToInput.addEventListener('keyup', e => {
    ordersFilter.to = e.target.value;
    updateOrdersList();
  });

  // Показываем весь список при первой инициализации фильтров
  updateRoutesList();
  updateOrdersList();
}

document.addEventListener('DOMContentLoaded', () => {
  loadData();
  setupDropdownFilters();
});


