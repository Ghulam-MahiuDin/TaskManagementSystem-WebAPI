document.getElementById('register-form').addEventListener('submit', async function (e) {
    e.preventDefault();

    const username = document.getElementById('register-username').value;
    const password = document.getElementById('register-password').value;

    const response = await fetch('/api/users/register', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ username, password })
    });

    if (response.ok) {
        alert('Registration successful. Please login.');
    } else {
        alert('Registration failed. Username may already be taken.');
    }
});

document.getElementById('login-form').addEventListener('submit', async function (e) {
    e.preventDefault();

    const username = document.getElementById('login-username').value;
    const password = document.getElementById('login-password').value;

    const response = await fetch('/api/users/login', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ username, password })
    });

    if (response.ok) {
        const data = await response.json();
        localStorage.setItem('token', data.token);
        document.getElementById('login').style.display = 'none';
        document.getElementById('register').style.display = 'none';
        document.getElementById('tasks').style.display = 'block';
        loadTasks();
    } else {
        alert('Login failed. Please check your credentials.');
    }
});

document.getElementById('task-form').addEventListener('submit', async function (e) {
    e.preventDefault();

    const title = document.getElementById('task-title').value;
    const description = document.getElementById('task-description').value;
    const dueDate = document.getElementById('task-due-date').value;
    const status = document.getElementById('task-status').value;

    const token = localStorage.getItem('token');

    const response = await fetch('/api/tasks', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify({ title, description, dueDate, status })
    });

    if (response.ok) {
        loadTasks();
    } else {
        alert('Failed to add task.');
    }
});

async function loadTasks() {
    const token = localStorage.getItem('token');

    const response = await fetch('/api/tasks', {
        method: 'GET',
        headers: {
            'Authorization': `Bearer ${token}`
        }
    });

    if (response.ok) {
        const tasks = await response.json();
        const taskList = document.getElementById('task-list');
        taskList.innerHTML = '';
        tasks.forEach(task => {
            const listItem = document.createElement('li');
            listItem.textContent = `${task.title}: ${task.description} (Due: ${task.dueDate}, Status: ${task.status})`;
            taskList.appendChild(listItem);
        });
    } else {
        alert('Failed to load tasks.');
    }
}
