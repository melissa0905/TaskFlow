import { memo } from 'react'
import type { TaskCardProps } from '../types/taskCard'
import { Link } from 'react-router'

const statusLabels: Record<number, string> = {
    1: 'Yapılacak',
    2: 'Devam ediyor',
    3: 'Tamamlandı',
}

export const TaskCard = memo(function TaskCard({
    task,
    selected,
    updating,
    onSelect,
    onStatusChange,
}: TaskCardProps) {
    return (
        <li className={`task-card ${selected ? 'task-card--selected' : ''}`}>
            <div className="task-card-header">
                <strong>{task.title}</strong>

                <span className={`status-badge status-${task.status}`}>
                    {statusLabels[task.status] ?? 'Bilinmeyen durum'}
                </span>
            </div>

            <p>{task.description}</p>

            <dl className="task-details">
                <div>
                    <dt>Proje</dt>
                    <dd>{task.projectName}</dd>
                </div>
                <div>
                    <dt>Atanan</dt>
                    <dd>{task.assigneeName ?? 'Henüz atanmadı'}</dd>
                </div>
                <div>
                    <dt>Son tarih</dt>
                    <dd>{task.dueDate.slice(0, 10)}</dd>
                </div>
            </dl>
            <div className="task-status-control">
                <label htmlFor={`status-${task.id}`}>Durum</label>

                <select
                    id={`status-${task.id}`}
                    value={task.status}
                    disabled={updating}
                    onChange={(event) => {
                        void onStatusChange(task.id, Number(event.target.value))
                    }}
                >
                    <option value={1}>Yapılacak</option>
                    <option value={2}>Devam ediyor</option>
                    <option value={3}>Tamamlandı</option>
                </select>

                {updating && <small>Güncelleniyor...</small>}
            </div>

            <button
                type="button"
                aria-pressed={selected}
                onClick={() => onSelect(task.id)}
            >
                {selected ? 'Seçildi' : 'Seç'}
            </button>
            <Link className="task-detail-link" to={`/tasks/${task.id}`}>
                Detayı aç →
            </Link>
        </li>
    )
})