import { useState } from 'react'

export default function Form({ userName, channelId, channelName, onSubmit }) {
  const [text, setText] = useState('')
  const [isSubmitting, setIsSubmitting] = useState(false)

  async function handleSubmit(e) {
    e.preventDefault()
    const trimmed = text.trim()
    if (!trimmed || isSubmitting) return

    setIsSubmitting(true)
    try {
      const message = {
        text: trimmed,
        userName,
        channelId,
      }
      const result = await onSubmit(message)
      if (result?.success) {
        setText('')
      }
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <div className="Form ">
      <form onSubmit={handleSubmit}>
        <div className="FormContents flex items-center sm:px-4">
          <input
            autoFocus
            maxLength={500}
            placeholder={`Chatting in ${channelName} as ${userName}`}
            type="text"
            id="text"
            name="text"
            value={text}
            onChange={(e) => setText(e.target.value)}
            disabled={isSubmitting}
            className="focus:outline-none grow bg-white dark:bg-black border rounded-md border-slate-500 shadow-md shadow-slate-400 dark:shadow-none w-5/6 mr-2 px-4 py-2 disabled:opacity-60"
          />
          <button
            type="submit"
            disabled={isSubmitting || !text.trim()}
            className="bg-orange-400 dark:bg-orange-600 hover:bg-orange-500 border rounded-md border-slate-500 shadow-md shadow-slate-400 dark:shadow-none px-3 py-2 disabled:opacity-50 disabled:cursor-not-allowed cursor-pointer"
          >
            {isSubmitting ? '⏳' : '📢'}
          </button>
        </div>
      </form>
    </div>
  )
}
