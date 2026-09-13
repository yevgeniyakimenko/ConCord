export default function Form({ userName, channelId, onSubmit }) {
  function handleSubmit(e) {
    e.preventDefault()
    const { text } = e.target.elements
    if (!text.value) return
    const message = {
      text: text.value,
      userName,
      channelId,
    }
    onSubmit(message)
    text.value = ''
  }

  return (
    <div className="Form ">
      <form onSubmit={handleSubmit}>
        <div className="FormContents flex items-center sm:px-4">
          <input
            autoFocus
            maxLength={500}
            placeholder={`Chatting as ${userName}`}
            type="text"
            id="text"
            name="text"
            className="focus:outline-none grow dark:bg-black border rounded-md border-slate-500 shadow-md shadow-slate-400 dark:shadow-none w-5/6 mr-2 px-4 py-2"
          />
          <button
            type="submit"
            className="bg-orange-400 dark:bg-orange-600 hover:bg-orange-500 border rounded-md border-slate-500 shadow-md shadow-slate-400 dark:shadow-none px-3 py-2"
          >
            📢
          </button>
        </div>
      </form>
    </div>
  )
}
