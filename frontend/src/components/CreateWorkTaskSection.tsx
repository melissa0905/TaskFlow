import { useEffect, useState, type SubmitEvent } from 'react'
import { getProjects } from '../api/projects'
import { createTeamMember, getTeamMembers } from '../api/teamMembers'
import type { TeamMember } from '../types/teamMember'
import { createWorkTask, getWorkTasks } from '../api/workTasks'
import type { Project } from '../types/project'
import type { WorkTask } from '../types/workTask'

export function CreateWorkTaskSection() {
    const [projects, setProjects] = useState<Project[]>([])
    const [members, setTeamMembers] = useState<TeamMember[]>([])
    const [tasks, setTasks] = useState<WorkTask[]>([])

    const [projectId, setProjectId] = useState('')
    const [title, setTitle] = useState('')
    const [description, setDescription] = useState('')
    const [dueDate, setDueDate] = useState('')
    const [assignedToId, setAssignedToId] = useState<string | null>(null)

    const [loading, setLoading] = useState(true)
    const [saving, setSaving] = useState(false)
    const [error, setError] = useState<string | null>(null)
    const [success, setSuccess] = useState(false)

    useEffect(() => {
        Promise.all([getProjects(), getTeamMembers(), getWorkTasks()])
            .then(([projectData, memberData, taskData]) => {
                setProjects(projectData)
                setTeamMembers(memberData)
                setTasks(taskData)
            })
            .catch((err) => {
                setError('Failed to fetch data')
            })
            .finally(() => setLoading(false))

    }, [])

    async function handleSubmit(event: SubmitEvent<HTMLFormElement>) {
        event.preventDefault()
        setError(null)
        setSuccess(false)
        setSaving(true)

        try {
            await createWorkTask({
                projectId,
                title: title.trim(),
                description: description.trim(),
                dueDate: `${dueDate}T00:00:00.000Z`,
                assignedToId: assignedToId || null,
            })
            setTasks(await getWorkTasks())
            setTitle('')
            setDescription('')
            setDueDate('')
            setAssignedToId('')
            setSuccess(true)

        } catch (err) {
            setError('Failed to create work task')
        } finally {
            setSaving(false)
        }
    }

    return (
        <section>
            <h2>Yeni İş Görevi Oluştur</h2>
            {loading && <p>Projeler ve ekip üyeleri yükleniyor...</p>}
            {error && <p role="alert">{error}</p>}
            {success && <p>Görev başarıyla oluşturuldu.</p>}
            {!loading && (
                <form onSubmit={handleSubmit}>
                    <div>
                        <label htmlFor="task-project">Project</label>
                        <select
                            id="task-project"
                            value={projectId}
                            onChange={(e) => setProjectId(e.target.value)}
                        >
                            <option value="">Select a project</option>
                            {projects.map((project) => (
                                <option key={project.id} value={project.id}>
                                    {project.name}
                                </option>
                            ))}
                        </select>
                    </div>

                    <div>
                        <label htmlFor="task-title">Title</label>
                        <input
                            type="text"
                            id="task-title"
                            value={title}
                            onChange={(e) => setTitle(e.target.value)}
                            maxLength={150}
                            required
                        />
                    </div>

                    <div>
                        <label htmlFor="task-description">Description</label>
                        <textarea
                            id="task-description"
                            value={description}
                            onChange={(e) => setDescription(e.target.value)}
                            maxLength={2000}
                            required
                        />
                    </div>

                    <div>
                        <label htmlFor="task-due-date">Due Date</label>
                        <input
                            type="date"
                            id="task-due-date"
                            value={dueDate}
                            onChange={(e) => setDueDate(e.target.value)}
                            required
                        />
                    </div>

                    <div>
                        <label htmlFor="task-assignee">Assigned To</label>
                        <select
                            id="task-assignee"
                            value={assignedToId || ''}
                            onChange={(e) => setAssignedToId(e.target.value || null)}
                        >
                            <option value="">Select a team member</option>
                            {members.map((member) => (
                                <option key={member.id} value={member.id}>
                                    {member.fullName} ({member.email})
                                </option>
                            ))}
                        </select>
                    </div>

                    <button type="submit" disabled={saving}>
                        {saving ? 'Creating...' : 'Create Work Task'}
                    </button>
                </form>
            )}
            <h2>Görevler</h2>

            {!loading && tasks.length === 0 && <p>Henüz görev yok.</p>}

            <ul>
                {tasks.map((task) => (
                    <li key={task.id}>
                        <strong>{task.title}</strong>
                        <p>{task.description}</p>
                        <p>Proje: {task.projectName}</p>
                        <p>Atanan: {task.assigneeName ?? 'Henüz atanmadı'}</p>
                        <p>Son tarih: {task.dueDate.slice(0, 10)}</p>
                        <p>Durum: {task.status === 1 ? 'Yapılacak' : task.status === 2 ? 'Devam ediyor' : 'Tamamlandı'}</p>
                    </li>
                ))}
            </ul>
        </section>
    )
}