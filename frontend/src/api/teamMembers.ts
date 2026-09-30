import type { TeamMember } from '../types/teamMember';

export async function getTeamMembers(): Promise<TeamMember[]> {
    const response = await fetch('/api/team-members');
    if (!response.ok) {
        throw new Error('Takım üyeleri yüklenemedi.')
    }
    return response.json() as Promise<TeamMember[]>
}
export async function createTeamMember(input: {
    fullName: string, email: string
}): Promise<void> {
    const response = await fetch('/api/team-members', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(input)
    })
    if (!response.ok) {
        throw new Error('Ekip üyesi eklenemedi.')
    }
}