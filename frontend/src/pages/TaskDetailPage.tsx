import { Link, useParams } from "react-router"
import type { WorkTaskDetail } from "../types/workTask"
import { useEffect, useState } from "react"
import { getWorkTaskById } from "../api/workTasks"
import { PageHeader } from "../components/PageHeader"

const statusLabels: Record<number, string> = {
    1: 'Yapılacak',
    2: 'Devam ediyor',
    3: 'Tamamlandı',
}

export default function TaskDetailPage() {
    const { id } = useParams<{ id: string }>()

    const [task, setTask] = useState<WorkTaskDetail | null>(null)
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState<string | null>(null)

    useEffect(() => {
        if (!id) return

        const controller = new AbortController()

        async function loadTask(taskId: string) {
            setLoading(true)
            setError(null)
            setTask(null)

            try {
                const data = await getWorkTaskById(taskId, controller.signal)

                if (!controller.signal.aborted) {
                    setTask(data)
                }
            } catch (err) {
                if (!controller.signal.aborted) {
                    setError(
                        err instanceof Error ? err.message : 'Bir hata oluştu.',
                    )
                }
            } finally {
                if (!controller.signal.aborted) {
                    setLoading(false)
                }
            }
        }

        void loadTask(id)

        return () => controller.abort()
    }, [id])

    return (
        <main>
            <PageHeader
                title={task?.title ?? 'Görev detayı'}
                description="Görev bilgilerini görüntüle ve düzenle."
            />
            <Link to="/tasks" className="back-link">Görevlere Dön</Link>
            <section className="task-detail-section">
                {!id && <p role="alert">Görev bulunamadı.</p>}
                {id && loading && <p>Görev yükleniyor...</p>}
                {error && <p role="alert">{error}</p>}
                {task && (
                    <>
                        <h2>{task.title}</h2>
                        <p>{task.description}</p>

                        <dl className="task-details">
                            <div>
                                <dt>Durum</dt>
                                <dd>{statusLabels[task.status] ?? 'Bilinmeyen'}</dd>
                            </div>

                            <div>
                                <dt>Son tarih</dt>
                                <dd>{task.dueDate.slice(0, 10)}</dd>
                            </div>
                        </dl>
                    </>
                )}
            </section>
        </main>
    )


}