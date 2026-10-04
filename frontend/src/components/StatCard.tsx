type StatCardProps = {
  title: string;
  value: number;
  description: string;
  tone?: "default" | "primary" | "success" | "warning" | "danger";
};

export default function StatCard({ title, value, description, tone = "default" }: StatCardProps) 
{
    return (
      <article className={`stat-card stat-card--${tone}`}>
        <h2  className="stat-card__title">{title}</h2>
        <p className="stat-card__value">{value}</p>
        <p className="stat-card__description">{description}</p>
      </article>  
    );
}
