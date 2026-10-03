import { useEffect, useState, type SubmitEvent } from 'react'
import { Link, useNavigate, useParams } from 'react-router'
import {
  deleteWorkTask,
  getWorkTaskById,
  updateWorkTask,
} from '../api/workTasks'
import { getTeamMembers } from '../api/teamMembers'
import { PageHeader } from '../components/PageHeader'
import type { TeamMember } from '../types/teamMember'

export default function TaskDetailPage() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()

  const [savedTitle, setSavedTitle] = useState('Görev detayı')
  const [title, setTitle] = useState('')
  const [description, setDescription] = useState('')
  const [dueDate, setDueDate] = useState('')
  const [assignedToId, setAssignedToId] = useState('')
  const [members, setMembers] = useState<TeamMember[]>([])

  const [loading, setLoading] = useState(true)
  const [loadError, setLoadError] = useState<string | null>(null)
  const [actionError, setActionError] = useState<string | null>(null)
  const [saving, setSaving] = useState(false)
  const [deleting, setDeleting] = useState(false)
  const [success, setSuccess] = useState(false)

  useEffect(() => {
    if (!id) return

    const controller = new AbortController()

    async function load(taskId: string) {
      setLoading(true)
      setLoadError(null)
      setActionError(null)
      setSuccess(false)

      try {
        const [task, teamMembers] = await Promise.all([
          getWorkTaskById(taskId, controller.signal),
          getTeamMembers(),
        ])

        if (controller.signal.aborted) return

        setSavedTitle(task.title)
        setTitle(task.title)
        setDescription(task.description)
        setDueDate(task.dueDate.slice(0, 10))
        setAssignedToId(task.assignedToId ?? '')
        setMembers(teamMembers)
      } catch (err) {
        if (!controller.signal.aborted) {
          setLoadError(
            err instanceof Error ? err.message : 'Veriler yüklenemedi.',
          )
        }
      } finally {
        if (!controller.signal.aborted) {
          setLoading(false)
        }
      }
    }

    void load(id)

    return () => controller.abort()
  }, [id])

  async function handleSubmit(event: SubmitEvent<HTMLFormElement>) {
    event.preventDefault()
    if (!id || saving || deleting) return

    const cleanTitle = title.trim()
    const cleanDescription = description.trim()

    if (!cleanTitle || !cleanDescription) {
      setActionError('Başlık ve açıklama boş olamaz.')
      return
    }

    setSaving(true)
    setSuccess(false)
    setActionError(null)

    try {
      await updateWorkTask(id, {
        title: cleanTitle,
        description: cleanDescription,
        dueDate: `${dueDate}T00:00:00.000Z`,
        assignedToId: assignedToId || null,
      })

      setTitle(cleanTitle)
      setDescription(cleanDescription)
      setSavedTitle(cleanTitle)
      setSuccess(true)
    } catch (err) {
      setActionError(
        err instanceof Error ? err.message : 'Kaydetme başarısız.',
      )
    } finally {
      setSaving(false)
    }
  }

  async function handleDelete() {
    if (!id || saving || deleting) return

    const confirmed = window.confirm(
      `"${savedTitle}" görevini kalıcı olarak silmek istiyor musun?`,
    )

    if (!confirmed) return

    setDeleting(true)
    setSuccess(false)
    setActionError(null)

    try {
      await deleteWorkTask(id)
      navigate('/tasks', { replace: true })
    } catch (err) {
      setActionError(
        err instanceof Error ? err.message : 'Silme başarısız.',
      )
    } finally {
      setDeleting(false)
    }
  }

  return (
    <main>
      <PageHeader
        title={savedTitle}
        description="Görevi düzenle veya sil."
      />

      <Link to="/tasks">← Görevlere dön</Link>

      <section className="task-detail-section">
        {!id && <p role="alert">Görev ID’si bulunamadı.</p>}
        {id && loading && <p>Görev yükleniyor...</p>}
        {loadError && <p role="alert">{loadError}</p>}

        {id && !loading && !loadError && (
          <>
            {actionError && <p role="alert">{actionError}</p>}
            {success && (
              <p className="success-message" role="status">
                Görev kaydedildi.
              </p>
            )}

            <form onSubmit={handleSubmit}>
              <div>
                <label htmlFor="edit-title">Başlık</label>
                <input
                  id="edit-title"
                  value={title}
                  onChange={(event) => {
                    setTitle(event.target.value)
                    setSuccess(false)
                  }}
                  maxLength={200}
                  disabled={saving || deleting}
                  required
                />
              </div>

              <div>
                <label htmlFor="edit-date">Son tarih</label>
                <input
                  id="edit-date"
                  type="date"
                  value={dueDate}
                  onChange={(event) => {
                    setDueDate(event.target.value)
                    setSuccess(false)
                  }}
                  disabled={saving || deleting}
                  required
                />
              </div>

              <div>
                <label htmlFor="edit-description">Açıklama</label>
                <textarea
                  id="edit-description"
                  value={description}
                  onChange={(event) => {
                    setDescription(event.target.value)
                    setSuccess(false)
                  }}
                  maxLength={2000}
                  disabled={saving || deleting}
                  required
                />
              </div>

              <div>
                <label htmlFor="edit-assignee">Atanan kişi</label>
                <select
                  id="edit-assignee"
                  value={assignedToId}
                  onChange={(event) => {
                    setAssignedToId(event.target.value)
                    setSuccess(false)
                  }}
                  disabled={saving || deleting}
                >
                  <option value="">Henüz atanmadı</option>
                  {members.map((member) => (
                    <option key={member.id} value={member.id}>
                      {member.fullName}
                    </option>
                  ))}
                </select>
              </div>

              <div className="form-actions">
                <button type="submit" disabled={saving || deleting}>
                  {saving ? 'Kaydediliyor...' : 'Değişiklikleri kaydet'}
                </button>

                <button
                  type="button"
                  className="button-danger"
                  onClick={handleDelete}
                  disabled={saving || deleting}
                >
                  {deleting ? 'Siliniyor...' : 'Görevi sil'}
                </button>
              </div>
            </form>
          </>
        )}
      </section>
    </main>
  )
}