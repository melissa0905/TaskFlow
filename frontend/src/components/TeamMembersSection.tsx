import { useEffect, useState, type SubmitEvent } from 'react'
import { createTeamMember, getTeamMembers } from '../api/teamMembers'
import type { TeamMember } from '../types/teamMember'
import { PageHeader } from './PageHeader'

function isAbortError(error: unknown) {
    return error instanceof DOMException && error.name === 'AbortError'
}

export function TeamMembersSection() {
    const [members, setTeamMembers] = useState<TeamMember[]>([])
    const [fullName, setFullName] = useState('')
    const [email, setEmail] = useState('')
    const [loading, setLoading] = useState(true)
    const [saving, setSaving] = useState(false)
    const [error, setError] = useState<string | null>(null)

    useEffect(() => {
        const controller = new AbortController()

        getTeamMembers(controller.signal)
            .then(setTeamMembers)
            .catch((err: unknown) => {
                if (isAbortError(err)) {
                    return
                }
                setError(err instanceof Error ? err.message : 'Bir hata oluştu.')
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
        setSaving(true)
        setError(null)
        try {
            await createTeamMember({ fullName: fullName.trim(), email: email.trim() })
            setFullName('')
            setEmail('')
            // Refresh the team members list
            const updatedMembers = await getTeamMembers()
            setTeamMembers(updatedMembers)
        } catch (err: unknown) {
            setError(err instanceof Error ? err.message : 'Bir hata oluştu.')
        } finally {
            setSaving(false)
        }
    }
    return (
        <section>
            <PageHeader
                title="Ekip Üyeleri"
                description="Ekip üyelerini oluştur ve yönet."
            />
            <form onSubmit={handleSubmit}>
                <div>
                    <label htmlFor="member-name">Ad Soyad:</label>
                    <input
                        id="member-name"
                        type="text"
                        maxLength={150}
                        value={fullName}
                        onChange={(e) => setFullName(e.target.value)}
                        required
                    />
                </div>
                <div>
                    <label htmlFor="email">E-posta:</label>
                    <input
                        id="email"
                        type="email"
                        value={email}
                        maxLength={255}
                        onChange={(e) => setEmail(e.target.value)}
                        required
                    />
                </div>
                <button type="submit" disabled={saving}>
                    {saving ? 'Ekleniyor...' : 'Ekip Üyesi Ekle'}
                </button>
            </form>
            {error && <p role="alert">{error}</p>}
            {loading && <p>Üyeler yükleniyor...</p>}

            {!loading && members.length === 0 && <p>Henüz üye yok.</p>}

            <ul>
                {members.map((member) => (
                    <li key={member.id}>
                        {member.fullName} — {member.email}
                    </li>
                ))}
            </ul>
        </section>
    )
}
