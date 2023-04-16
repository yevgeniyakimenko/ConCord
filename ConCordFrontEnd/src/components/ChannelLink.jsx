export default function ChannelLink({ channel, selected, onSelect }) {
  const selCh = selected ? 'font-extrabold text-orange-600 dark:text-orange-500 hover:text-orange-600 hover:dark:text-orange-500' : ''
  const handleClick = () => {
    onSelect(channel)
  }
  return (
    <li className={`mb-3 cursor-pointer break-all hover:text-orange-500 hover:font-bold ${selCh}`} onClick={handleClick}>
      {selected ? '🔥 ' : ''}
      {channel.name}
    </li>
  )
}