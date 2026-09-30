import type { CreateProjectInput, Project } from '../types/project';

export async function getProjects(signal?: AbortSignal): Promise<Project[]> {
    const response = await fetch('/api/projects', { signal });
    if (!response.ok) {
        throw new Error('Projeler yüklenemedi.')
    }
    return response.json() as Promise<Project[]>
}

export async function createProject(input: CreateProjectInput): Promise<void> {
    const response = await fetch('/api/projects', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(input),
    });
    if (!response.ok) {
        throw new Error('Proje oluşturulamadı.')
    }
}
