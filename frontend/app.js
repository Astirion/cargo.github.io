'use strict';

let travelers = [];
let senders = [];

const travelerForm = document.getElementById('travelerForm');
const senderForm = document.getElementById('senderForm');
const matchList = document.getElementById('matchList');
const noMatches = document.getElementById('noMatches');

async function loadData() {
  try {
    const apiBase = 'http://localhost:5115';
    const [travelersRes, sendersRes] = await Promise.all([
      fetch(`${apiBase}/api/travelers`).then(r => r.json()),
      fetch(`${apiBase}/api/senders`).then(r => r.json())
    ]);
    travelers = travelersRes;
    senders = sendersRes;
    findMatches();
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
    const apiBase = 'http://localhost:5115';
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
    const apiBase = 'http://localhost:5115';
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
      const toMatch = sender.to.toLowerCase() === traveler.to.toLowerCase();
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

document.addEventListener('DOMContentLoaded', loadData);


