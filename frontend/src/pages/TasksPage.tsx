import { PageHeader } from '../components/PageHeader'
import { CreateWorkTaskSection } from '../components/CreateWorkTaskSection'

export default function TasksPage() {
  return (
    <main>
      <PageHeader
        title="Görevler"
        description="Görev oluştur, projeye bağla ve ekip üyesine ata."
      />

      <CreateWorkTaskSection />
    </main>
  )
}