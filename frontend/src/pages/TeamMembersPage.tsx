import { PageHeader } from '../components/PageHeader'
import { TeamMembersSection } from '../components/TeamMembersSection'

export default function TeamMembersPage() {
  return (
    <main>
      <PageHeader
        title="Ekip"
        description="Ekip üyelerini görüntüle ve yeni üyeler ekle."
      />

      <TeamMembersSection />
    </main>
  )
}