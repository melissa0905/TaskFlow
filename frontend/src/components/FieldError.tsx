type FieldErrorProps = {
  id: string;
  messages?: string[];
};

export default function FieldError({
  id,
  messages,
}: FieldErrorProps) {
  if (!messages?.length) {
    return null;
  }

  return (
    <div id={id} className="field-error">
      {messages.map((message, index) => (
        <p key={`${index}-${message}`}>{message}</p>
      ))}
    </div>
  );
}