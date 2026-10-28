import { useEffect, useState } from 'react';
import type { SubmitEvent } from 'react';

import { getProjects } from '../api/projects';
import { useTaskPlanner } from '../hooks/useTaskPlanner';

import type { TaskCategory } from '../types/taskPlan';
import { PageHeader } from '../components/PageHeader';

type ProjectOption = {
  id: string;
  name: string;
};

const categoryLabels: Record<TaskCategory, string> = {
  backend: 'Backend',
  frontend: 'Frontend',
  testing: 'Test',
  devops: 'DevOps',
};

export default function AiPlannerPage() {
  const [projects, setProjects] = useState<ProjectOption[]>([]);
  const [projectsLoading, setProjectsLoading] = useState(true);
  const [projectsError, setProjectsError] = useState('');

  const [projectId, setProjectId] = useState('');
  const [goal, setGoal] = useState('');
  const [maxTasks, setMaxTasks] = useState(3);
  const [formError, setFormError] = useState('');

  const {
    plan,
    loading,
    error,
    generate,
    cancel,
    reset,
  } = useTaskPlanner();

  useEffect(() => {
    let active = true;

    async function loadProjects() {
      try {
        const result = await getProjects();

        if (active) {
          setProjects(result);
        }
      } catch (error: unknown) {
        if (active) {
          setProjectsError(
            error instanceof Error
              ? error.message
              : 'Projeler yüklenemedi.',
          );
        }
      } finally {
        if (active) {
          setProjectsLoading(false);
        }
      }
    }

    void loadProjects();

    return () => {
      active = false;
    };
  }, []);

  async function handleSubmit(
    event: SubmitEvent<HTMLFormElement>,
  ): Promise<void> {
    event.preventDefault();

    if (loading) {
      return;
    }

    setFormError('');

    const trimmedGoal = goal.trim();

    if (!projectId) {
      setFormError('Bir proje seçmelisin.');
      return;
    }

    if (trimmedGoal.length < 10 || trimmedGoal.length > 2000) {
      setFormError('Hedef metni 10–2000 karakter arasında olmalıdır.');
      return;
    }

    if (!Number.isInteger(maxTasks) || maxTasks < 1 || maxTasks > 10) {
      setFormError('Görev sayısı 1–10 arasında bir tam sayı olmalıdır.');
      return;
    }

    await generate({
      project_id: projectId,
      goal: trimmedGoal,
      max_tasks: maxTasks,
    });
  }

  function clearPreviousResult() {
    setFormError('');
    reset();
  }

  return (
    <main>
      <PageHeader
        title="AI Planlayıcı"
        description="Hedefini yaz, projen için görev önerileri hazırla."
      />

      <section className="ai-panel">
        {projectsError && (
          <p className="form-error" role="alert">
            {projectsError}
          </p>
        )}

        <form
          className="ai-planner-form"
          onSubmit={handleSubmit}
        >
          <label className="ai-form-field">
            Proje

            <select
              value={projectId}
              required
              disabled={projectsLoading || loading}
              onChange={(event) => {
                setProjectId(event.target.value);
                clearPreviousResult();
              }}
            >
              <option value="">
                {projectsLoading ? 'Projeler yükleniyor...' : 'Proje seç'}
              </option>

              {projects.map((project) => (
                <option key={project.id} value={project.id}>
                  {project.name}
                </option>
              ))}
            </select>
          </label>

          <label className="ai-form-field">
            Ne geliştirmek istiyorsun?

            <textarea
              value={goal}
              required
              minLength={10}
              maxLength={2000}
              rows={5}
              disabled={loading}
              placeholder="Örneğin: Projeye kullanıcı girişi ve yetkilendirme eklemek istiyorum."
              onChange={(event) => {
                setGoal(event.target.value);
                clearPreviousResult();
              }}
            />

            <small>{goal.length} / 2000 karakter</small>
          </label>

          <label className="ai-form-field">
            En fazla kaç görev önerilsin?

            <input
              type="number"
              min={1}
              max={10}
              step={1}
              required
              value={Number.isNaN(maxTasks) ? '' : maxTasks}
              disabled={loading}
              onChange={(event) => {
                setMaxTasks(event.target.valueAsNumber);
                clearPreviousResult();
              }}
            />
          </label>

          <div className="ai-actions">
            <button
              type="submit"
              disabled={
                loading ||
                projectsLoading ||
                projects.length === 0
              }
            >
              {loading ? 'Plan hazırlanıyor...' : 'Görev planı oluştur'}
            </button>

            {loading && (
              <button type="button" onClick={cancel}>
                İptal et
              </button>
            )}
          </div>
        </form>

        {loading && (
          <p role="status">
            Proje bilgileri inceleniyor ve görev önerileri hazırlanıyor…
          </p>
        )}

        {(formError || error) && (
          <p className="form-error" role="alert">
            {formError || error}
          </p>
        )}
      </section>

      {plan && (
        <section className="ai-results">
          <h2>Önerilen görev planı</h2>

          <p className="ai-summary">{plan.summary}</p>

          <p className="ai-draft-note">
            Bu öneriler taslaktır. Henüz görev olarak kaydedilmedi.
          </p>

          <ol className="ai-proposal-list">
            {plan.tasks.map((task, index) => (
              <li className="ai-proposal-card" key={index}>
                <span className="ai-category">
                  {categoryLabels[task.category]}
                </span>

                <h3>{task.title}</h3>

                <p>{task.description}</p>
              </li>
            ))}
          </ol>
        </section>
      )}
    </main>
  );
}