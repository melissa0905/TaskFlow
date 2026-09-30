import { useEffect, useState, type SubmitEvent } from 'react'
import './App.css'
import { createProject, getProjects } from './api/projects'
import type { Project } from './types/project'
import { TeamMembersSection } from './components/TeamMembersSection'
import { CreateWorkTaskSection } from './components/CreateWorkTaskSection'

function App() {
  const [projects, setProjects] = useState<Project[]>([])
  const [name, setName] = useState('')
  const [description, setDescription] = useState('')
  const [loading, setLoading] = useState(true)
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState<string | null>(null)

  async function loadProjects() {
    const data = await getProjects()
    setProjects(data)
  }

  useEffect(() => {
    getProjects()
      .then(setProjects)
      .catch((err: unknown) => {
        setError(err instanceof Error ? err.message : 'Bir hata oluştu.')
      })
      .finally(() => setLoading(false))
  }, [])

  async function handleSubmit(event: SubmitEvent<HTMLFormElement>) {
    event.preventDefault()
    if (!name.trim()) {
      setError('Proje adı boş bırakılamaz.')
      return
    }
    setSaving(true)
    try {
      await createProject({ name: name.trim(), description: description.trim() })
      setName('')
      setDescription('')
      await loadProjects()
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Bir hata oluştu.')
    } finally {
      setSaving(false)
    }
  }



  return (
    <main>
      <header className="page-header">
        <h1>TaskFlow</h1>
        <p>Projelerini, ekibini ve görevlerini tek yerden yönet.</p>
      </header>
      <section>
        <h2>Yeni Proje</h2>
        <form onSubmit={handleSubmit}>
          <div>
            <label htmlFor="project-name">Proje Adı</label>
            <input
              id="project-name"
              type="text"
              value={name}
              onChange={(e) => setName(e.target.value)}
              maxLength={150}
              required
            />
          </div>
          <div>
            <label htmlFor="description">Açıklama</label>
            <textarea
              id="project-description"
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              maxLength={1000}
              rows={3}
            />
          </div>
          <button type="submit" disabled={saving}>
            {saving ? 'Kaydediliyor...' : 'Proje Oluştur'}
          </button>
        </form>
      </section>

      <section>
        <h2>Projeler</h2>
        {error && <p role="alert" style={{ color: 'red' }}>{error}</p>}
        {loading && <p>Projeler Yükleniyor...</p>}
        {!loading && projects.length === 0 && (
          <p>Henüz proje eklenmedi.</p>
        )}
        <ul>
          {projects.map((project) => (
            <li key={project.id}>
              <h3>{project.name}</h3>
              {project.description && <p>{project.description}</p>}
            </li>
          ))}
        </ul>
      </section>
      <TeamMembersSection />
      <CreateWorkTaskSection />
    </main>
  )
}

export default App
