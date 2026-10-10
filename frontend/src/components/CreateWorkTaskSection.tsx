import { useEffect, useState, type SubmitEvent } from 'react'
import { getProjects } from '../api/projects'
import { getTeamMembers } from '../api/teamMembers'
import type { TeamMember } from '../types/teamMember'
import { createWorkTask } from '../api/workTasks'
import type { Project } from '../types/project'
import { ApiError } from '../api/ApiError';
import type { FieldErrors } from '../api/ApiError';
import FieldError from './FieldError';

type CreateWorkTaskSectionProps = {
    onCreated: () => Promise<void>
}
function isAbortError(error: unknown) {
    return error instanceof DOMException && error.name === 'AbortError'
}

export function CreateWorkTaskSection({
    onCreated,
}: CreateWorkTaskSectionProps) {
    const [projects, setProjects] = useState<Project[]>([])
    const [members, setTeamMembers] = useState<TeamMember[]>([])

    const [projectId, setProjectId] = useState('')
    const [title, setTitle] = useState('')
    const [description, setDescription] = useState('')
    const [dueDate, setDueDate] = useState('')
    const [assignedToId, setAssignedToId] = useState<string | null>(null)

    const [loading, setLoading] = useState(true)
    const [saving, setSaving] = useState(false)
    const [fieldErrors, setFieldErrors] = useState<FieldErrors>({});
    const [error, setError] = useState<string | null>(null)
    const [success, setSuccess] = useState(false)

    useEffect(() => {
        const controller = new AbortController()

        Promise.all([
            getProjects(controller.signal),
            getTeamMembers(controller.signal),
        ])
            .then(([projectData, memberData]) => {
                setProjects(projectData)
                setTeamMembers(memberData)
            })
            .catch((err: unknown) => {
                if (isAbortError(err)) {
                    return
                }
                setError('Failed to fetch data')
            })
            .finally(() => {
                if (!controller.signal.aborted) {
                    setLoading(false)
                }
            })

        return () => controller.abort()
    }, [])

    async function handleSubmit(event: SubmitEvent<HTMLFormElement>) {
        event.preventDefault()
        setSuccess(false)
        setSaving(true)
        setError('');
        setFieldErrors({});

        try {
            await createWorkTask({
                projectId,
                title: title.trim(),
                description: description.trim(),
                dueDate: `${dueDate}T00:00:00.000Z`,
                assignedToId: assignedToId || null,
            })
            setTitle('')
            setDescription('')
            setDueDate('')
            setAssignedToId('')
            setSuccess(true)
            await onCreated()

        } catch (error: unknown) {
            if (error instanceof ApiError) {
                setFieldErrors(error.fieldErrors);
                setError(error.message);
            } else {
                setError(
                    error instanceof Error
                        ? error.message
                        : 'Beklenmeyen bir hata oluştu.',
                );
            }
        } finally {
            setSaving(false)
        }
    }

    return (
        <section>
            <h2>Yeni İş Görevi Oluştur</h2>
            {loading && <p>Projeler ve ekip üyeleri yükleniyor...</p>}
            {error && (
                <p className="form-error" role="alert">
                    {error}
                </p>
            )}
            {success && <p>Görev başarıyla oluşturuldu.</p>}
            {!loading && (
                <form onSubmit={handleSubmit}>
                    <div>
                        <label htmlFor="task-project">Proje</label>
                        <select
                            id="task-project"
                            value={projectId}
                            onChange={(e) => setProjectId(e.target.value)}
                        >
                            <option value="">Bir proje seçin</option>
                            {projects.map((project) => (
                                <option key={project.id} value={project.id}>
                                    {project.name}
                                </option>
                            ))}
                        </select>
                        <FieldError
                            id="task-project-error"
                            messages={fieldErrors.projectId}
                        />
                    </div>

                    <div>
                        <label htmlFor="task-title">Başlık</label>
                        <input
                            type="text"
                            id="task-title"
                            value={title}
                            onChange={(e) => setTitle(e.target.value)}
                            aria-invalid={Boolean(fieldErrors.title?.length)}
                            aria-describedby={
                                fieldErrors.title?.length
                                    ? 'task-title-error'
                                    : undefined
                            }
                            maxLength={150}
                            required
                        />
                        <FieldError
                            id="task-title-error"
                            messages={fieldErrors.title}
                        />
                    </div>

                    <div>
                        <label htmlFor="task-description">Açıklama</label>
                        <textarea
                            id="task-description"
                            value={description}
                            onChange={(e) => setDescription(e.target.value)}
                            maxLength={2000}
                            required
                        />
                        <FieldError
                            id="task-description-error"
                            messages={fieldErrors.description}
                        />
                    </div>

                    <div>
                        <label htmlFor="task-due-date">Son Tarih</label>
                        <input
                            type="date"
                            id="task-due-date"
                            value={dueDate}
                            onChange={(e) => setDueDate(e.target.value)}
                            required
                        />
                        <FieldError
                            id="task-due-date-error"
                            messages={fieldErrors.dueDate}
                        />
                    </div>

                    <div>
                        <label htmlFor="task-assignee">Atanan</label>
                        <select
                            id="task-assignee"
                            value={assignedToId || ''}
                            onChange={(e) => setAssignedToId(e.target.value || null)}
                        >
                            <option value="">Bir ekip üyesi seçin</option>
                            {members.map((member) => (
                                <option key={member.id} value={member.id}>
                                    {member.fullName} ({member.email})
                                </option>
                            ))}
                        </select>
                        <FieldError
                            id="task-assignee-error"
                            messages={fieldErrors.assignedToId}
                        />
                    </div>

                    <button type="submit" disabled={saving}>
                        {saving ? 'Creating...' : 'Görev Oluştur'}
                    </button>
                </form>
            )}

        </section>
    )
}
