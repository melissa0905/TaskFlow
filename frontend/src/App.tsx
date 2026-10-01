import { Navigate, NavLink, Route, Routes } from 'react-router'
import TeamMembersPage from './pages/TeamMembersPage'
import ProjectsPage from './pages/ProjectsPage'
import TasksPage from './pages/TasksPage'
import './App.css'
import TaskDetailPage from './pages/TaskDetailPage'
function App() {
  return (
    <>
      <nav className="app-nav" aria-label="Ana menü">
        <span className="app-brand">TaskFlow</span>

        <NavLink to="/projects">Projeler</NavLink>
        <NavLink to="/team">Ekip</NavLink>
        <NavLink to="/tasks">Görevler</NavLink>
      </nav>

      <Routes>
        <Route path="/" element={<Navigate to="/projects" replace />} />
        <Route path="/projects" element={<ProjectsPage />} />
        <Route path="/team" element={<TeamMembersPage />} />
        <Route path="/tasks" element={<TasksPage />} />
        <Route path="/tasks/:id" element={<TaskDetailPage />} />
        <Route
          path="*"
          element={
            <main>
              <h1>Sayfa bulunamadı</h1>
              <NavLink to="/projects">Projelere dön</NavLink>
            </main>
          }
        />
      </Routes>
    </>
  )
}

export default App