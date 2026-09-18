export default function Message({ message, username }) {
  const options = {
    hour: 'numeric',
    minute: 'numeric',
    second: 'numeric',
    month: 'long',
    day: 'numeric',
    year: 'numeric',
  }
  const myMessage =
    message.userName === username
      ? 'text-orange-600 dark:text-orange-500'
      : 'text-orange-800 dark:text-orange-700'

  return (
    <>
      <li className="mb-2">
        <p className="flex items-start">
          <span className={`mr-2 block shrink-0 ${myMessage}`}>
            <span className="font-semibold">{message.userName}</span>:
          </span>

          <span className="block">{message.text}</span>
        </p>

        <p className="text-xs text-slate-400">
          {new Date(message.created).toLocaleDateString('en-US', options)}
        </p>
      </li>
    </>
  )
}
