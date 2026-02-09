// API Configuration
const API_BASE_URL = 'http://localhost:5000/api/todos';

// DOM Elements
const apiStatusElement = document.getElementById('apiStatus');
const todoForm = document.getElementById('todoForm');
const formTitle = document.getElementById('formTitle');
const submitBtn = document.getElementById('submitBtn');
const cancelBtn = document.getElementById('cancelBtn');
const todoContainer = document.getElementById('todoContainer');
const todoCountElement = document.getElementById('todoCount');
const notificationContainer = document.getElementById('notificationContainer');

// State
let editingTodoId = null;
let todos = [];

// Initialize
document.addEventListener('DOMContentLoaded', init);

function init() {
    checkApiConnection();
    loadTodos();
    setupEventListeners();
}

// Check if API is running
async function checkApiConnection() {
    try {
        const response = await fetch(API_BASE_URL);
        if (response.ok) {
            apiStatusElement.textContent = '✅ API: Connected';
            apiStatusElement.className = 'api-status status-connected';
        } else {
            throw new Error('API not responding');
        }
    } catch (error) {
        apiStatusElement.textContent = '❌ API: Disconnected - Start the .NET API first!';
        apiStatusElement.className = 'api-status status-disconnected';
    }
}

// Setup event listeners
function setupEventListeners() {
    todoForm.addEventListener('submit', handleSubmit);
    cancelBtn.addEventListener('click', cancelEdit);
}

// Handle form submission
async function handleSubmit(e) {
    e.preventDefault();
    
    const title = document.getElementById('title').value.trim();
    const description = document.getElementById('description').value.trim();
    const isCompleted = document.getElementById('isCompleted').checked;
    
    if (!title) {
        showNotification('Title is required!', 'error');
        return;
    }
    
    const todoData = {
        title,
        description,
        isCompleted
    };
    
    try {
        if (editingTodoId) {
            await updateTodo(editingTodoId, todoData);
            showNotification('Todo updated successfully!', 'success');
        } else {
            await createTodo(todoData);
            showNotification('Todo created successfully!', 'success');
        }
        
        resetForm();
        await loadTodos();
    } catch (error) {
        showNotification(`Error: ${error.message}`, 'error');
        console.error('Error saving todo:', error);
    }
}

// Load all todos
async function loadTodos() {
    try {
        todoContainer.innerHTML = '<div class="loading">Loading todos...</div>';
        
        const response = await fetch(API_BASE_URL);
        
        if (!response.ok) {
            throw new Error(`API returned ${response.status}`);
        }
        
        todos = await response.json();
        renderTodos();
        
        // Update count
        todoCountElement.textContent = todos.length;
        
    } catch (error) {
        todoContainer.innerHTML = `
            <div class="error">
                ❌ Error loading todos: ${error.message}<br>
                Make sure the .NET API is running at ${API_BASE_URL}
            </div>
        `;
        todoCountElement.textContent = '0';
    }
}

// Render todos to the DOM
function renderTodos() {
    if (todos.length === 0) {
        todoContainer.innerHTML = '<div class="no-todos">No todos yet. Add your first todo above!</div>';
        return;
    }
    
    const todoItems = todos.map(todo => {
        const formattedDate = formatDate(todo.createdAt);
        const statusClass = todo.isCompleted ? 'status-completed' : 'status-pending';
        const statusText = todo.isCompleted ? '✅ Completed' : '⏳ Pending';
        
        return `
            <div class="todo-item" data-id="${todo.id}">
                <div class="todo-content">
                    <div class="todo-title">${escapeHtml(todo.title)}</div>
                    ${todo.description ? `<div class="todo-description">${escapeHtml(todo.description)}</div>` : ''}
                    <div class="todo-meta">
                        <span>Created: ${formattedDate}</span>
                        <span class="todo-status ${statusClass}">${statusText}</span>
                    </div>
                </div>
                <div class="todo-actions">
                    <button class="btn-edit" onclick="startEditTodo(${todo.id})">Edit</button>
                    <button class="btn-delete" onclick="deleteTodo(${todo.id})">Delete</button>
                </div>
            </div>
        `;
    }).join('');
    
    todoContainer.innerHTML = todoItems;
}

// Create a new todo
async function createTodo(todoData) {
    const response = await fetch(API_BASE_URL, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(todoData)
    });
    
    if (!response.ok) {
        const error = await response.json();
        throw new Error(error.message || 'Failed to create todo');
    }
    
    return await response.json();
}

// Update existing todo
async function updateTodo(id, todoData) {
    const response = await fetch(`${API_BASE_URL}/${id}`, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(todoData)
    });
    
    if (!response.ok) {
        const error = await response.json();
        throw new Error(error.message || 'Failed to update todo');
    }
    
    return await response.json();
}

// Delete todo
async function deleteTodo(id) {
    if (!confirm('Are you sure you want to delete this todo?')) {
        return;
    }
    
    try {
        const response = await fetch(`${API_BASE_URL}/${id}`, {
            method: 'DELETE'
        });
        
        if (!response.ok) {
            throw new Error('Failed to delete todo');
        }
        
        showNotification('Todo deleted successfully!', 'success');
        await loadTodos();
    } catch (error) {
        showNotification(`Error: ${error.message}`, 'error');
    }
}

// Start editing a todo
function startEditTodo(id) {
    const todo = todos.find(t => t.id === id);
    if (!todo) return;
    
    editingTodoId = id;
    
    // Fill form with todo data
    document.getElementById('title').value = todo.title;
    document.getElementById('description').value = todo.description || '';
    document.getElementById('isCompleted').checked = todo.isCompleted;
    
    // Update UI for editing mode
    formTitle.textContent = '✏️ Edit Todo';
    submitBtn.textContent = 'Update Todo';
    cancelBtn.style.display = 'inline-block';
    
    // Scroll to form
    document.querySelector('.todo-form').scrollIntoView({ behavior: 'smooth' });
}

// Cancel edit mode
function cancelEdit() {
    editingTodoId = null;
    resetForm();
}

// Reset form to add mode
function resetForm() {
    todoForm.reset();
    editingTodoId = null;
    formTitle.textContent = '➕ Add New Todo';
    submitBtn.textContent = 'Add Todo';
    cancelBtn.style.display = 'none';
}

// Show notification
function showNotification(message, type = 'success') {
    const notification = document.createElement('div');
    notification.className = `notification ${type}`;
    notification.textContent = message;
    
    notificationContainer.appendChild(notification);
    
    // Remove notification after 3 seconds
    setTimeout(() => {
        notification.remove();
    }, 3000);
}

// Utility: Format date
function formatDate(dateString) {
    const date = new Date(dateString);
    return date.toLocaleDateString() + ' ' + date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
}

// Utility: Escape HTML to prevent XSS
function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

// Make functions available globally
window.deleteTodo = deleteTodo;
window.startEditTodo = startEditTodo;