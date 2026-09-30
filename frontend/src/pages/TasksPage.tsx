import { useCallback, useMemo, useState } from 'react'
import { PageHeader } from '../components/PageHeader'
import { CreateWorkTaskSection } from '../components/CreateWorkTaskSection'
import { TaskCard } from '../components/TaskCard'
import { useTasks } from '../hooks/useTasks'
import { changeWorkTaskStatus } from '../api/workTasks'

export default function TasksPage() {
  const { tasks, loading, error, reload } = useTasks()
  const [search, setSearch] = useState('')
  const [status, setStatus] = useState('all')
  const [selectedId, setSelectedId] = useState<string | null>(null)

  const [updatingId, setUpdatingId] = useState<string | null>(null)
  const [updateError, setUpdateError] = useState<string | null>(null)

  const filteredTasks = useMemo(() => {
    const term = search.toLocaleLowerCase('tr-TR')

    return tasks.filter((task) => {
      const matchesSearch = task.title.toLocaleLowerCase('tr-TR').includes(term)
      const matchesStatus = status === 'all' || task.status === Number(status)

      return matchesSearch && matchesStatus
    })
  }, [tasks, search, status])

  const handleSelect = useCallback((id: string) => {
    setSelectedId((current) => current === id ? null : id)
  }, [])
  const handleStatusChange = useCallback(
    async (id: string, status: number) => {
      setUpdatingId(id)
      setUpdateError(null)

      try {
        await changeWorkTaskStatus(id, status)
        await reload()
      } catch (err) {
        setUpdateError(
          err instanceof Error ? err.message : 'Bir hata oluştu.',
        )
      } finally {
        setUpdatingId(null)
      }
    },
    [reload],
  )
  return (<main>
    <PageHeader
      title="Görevler"
      description="Görevlerini oluştur, filtrele ve takip et."
    />

    <CreateWorkTaskSection onCreated={reload} />

    <section>
      <h2>Görev listesi ({filteredTasks.length})</h2>

      <div className="task-filters">
        <div>
          <label htmlFor="task-search">Başlıkta ara</label>
          <input
            id="task-search"
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            placeholder="Görev başlığı..."
          />
        </div>

        <div>
          <label htmlFor="task-status-filter">Durum</label>
          <select
            id="task-status-filter"
            value={status}
            onChange={(event) => setStatus(event.target.value)}
          >
            <option value="all">Tümü</option>
            <option value="1">Yapılacak</option>
            <option value="2">Devam ediyor</option>
            <option value="3">Tamamlandı</option>
          </select>
        </div>
      </div>

      {loading && <p>Görevler yükleniyor...</p>}
      {error && <p role="alert">{error}</p>}
      {updateError && <p role="alert">{updateError}</p>}
      {!loading && !error && filteredTasks.length === 0 && (
        <p>Filtreye uygun görev bulunamadı.</p>
      )}

      <ul>
        {filteredTasks.map((task) => (
          <TaskCard
            key={task.id}
            task={task}
            selected={selectedId === task.id}
            updating={updatingId !== null}
            onSelect={handleSelect}
            onStatusChange={handleStatusChange}
          />
        ))}
      </ul>
    </section>
  </main>)
}