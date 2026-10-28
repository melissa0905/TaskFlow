export type TaskCategory =
    | 'backend'
    | 'frontend'
    | 'testing'
    | 'devops'

export interface CreateTaskPlanRequest {
    project_id: string;
    goal: string;
    max_tasks: number;
}

export interface TaskProposal {
    title: string;
    description: string;
    category: TaskCategory;
}

export interface TaskPlanResponse {
    project_id: string;
    summary: string;
    tasks: TaskProposal[];
}