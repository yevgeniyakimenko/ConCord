import "./App.css"
import { useState } from "react"

import Chat from './components/Chat'

export default function App() {
  const [username, setUsername] = useState(null)

  return (
    <div className="App h-full flex flex-col sm:flex-row">
      {username ? 
      <Chat username={username} />
      : 
      <div className='flex flex-col items-center justify-center w-full h-full'>
        <form 
          className='flex flex-col sm:flex-row items-center justify-center sm:w-full'
          onSubmit={(e) => {
            e.preventDefault()
            if (!e.target.username.value) return
            let userName = e.target.username.value.slice(0, 10)
            setUsername(userName)
          }}
        >
          <label htmlFor="username" className='text-xl sm:mr-2 mb-4'>Your username:</label>
          <input
            autoFocus
            maxLength={10}
            type="text"
            id="username"
            name="username"
            className='focus:outline-none dark:bg-black border rounded-md border-slate-500 shadow-md shadow-slate-400 dark:shadow-zinc-600 mb-4 sm:mr-4 px-4 py-2'
          />
          <button
            type="submit"
            className='focus:outline-none bg-orange-400 dark:bg-orange-600 hover:bg-orange-500 border rounded-md border-slate-500 shadow-md shadow-slate-400 dark:shadow-zinc-600 text-white font-bold mb-4 py-2 px-4'
          >
            ✔️
          </button>
        </form>
      </div>}
    </div>
  )
}