export default function Message({ message, username }) {
  const options = { hour: 'numeric', minute: 'numeric', second: 'numeric', month: 'long', day: 'numeric', year: 'numeric' }
  const myMessage = message.userName === username ? 'text-orange-600 dark:text-orange-500' : 'text-orange-800 dark:text-orange-700'
  
  let wordArr = message.text.split(' ').map((word) => {
    return word.length > 24 ? word.match(/.{1,24}/g) : word
  })
  let newWordArr = wordArr.flat()
  const text = newWordArr.join(' ')

  return (
    <>
      <li className='mb-2'>
        <p className=''>
          <span className={`mr-2 ${myMessage}`}>
            <span className="font-semibold">
              {message.userName}
            </span>:
          </span>
          {text}
        </p>

        <p className='text-xs text-slate-400'>
          {new Date(message.created).toLocaleDateString('en-US', options)}
        </p>
      </li>
    </>
  )
}