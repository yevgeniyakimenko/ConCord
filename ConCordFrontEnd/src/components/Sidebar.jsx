import { useState } from 'react'
import ChannelLink from './ChannelLink'

export default function Sidebar({ channels, selectedChannel, onSelectChannel, onSubmitChannel }) {

  const [input, setInput] = useState('')
  return (
    <div className="sidebar text-left sm:w-1/6 pb-4 sm:pt-4 flex flex-col">
      <h1 className='text-2xl font-semibold text-orange-600 dark:text-orange-500 mb-3'>
        {'ConCord '}
        {/* <br className='hidden sm:inline' /> */}
        {'Chat'}
      </h1>

      {/* a select for channels */}
      {selectedChannel && <select
        value={selectedChannel.id}
        onChange={(e) => {
          const newSelChannel = channels.find((channel) => channel.id == e.target.value)
          onSelectChannel(newSelChannel)
        }}
        className='sm:hidden border bg-white dark:bg-black rounded-md border-slate-500 shadow-md shadow-slate-400 dark:shadow-zinc-600 px-4 py-3 mb-4'
      >
        {channels && channels.map((channel) => (
          <option key={channel.id} value={channel.id}>
            {channel.name}
          </option>
        ))}
      </select>}


      <ul className='channelList hidden sm:block'>
        {channels && channels.map((channel) => (
          <ChannelLink
            key={channel.id}
            channel={channel}
            selected={channel.id === selectedChannel.id}
            onSelect={onSelectChannel}
          />
        ))}
      </ul>
      
        <form>
          <div className='flex sm:flex-col sm:items:'>
          <input
            type="text"
            maxLength={50}
            value={input}
            placeholder="New channel name"
            onChange={(e) => setInput(e.target.value)}
            className='sm:block grow border rounded-md focus:outline-none dark:bg-black border-slate-500 shadow-md shadow-slate-400 dark:shadow-zinc-600 px-4 py-2 mr-2 sm:mr-0 sm:mb-2'
          />
          <button
            type="button"
            className='sm:w-16 border rounded-md bg-white dark:bg-black hover:bg-orange-200 hover:dark:bg-orange-900 border-slate-500 shadow-md shadow-slate-400 dark:shadow-zinc-600 px-3 py-2'
            onClick={async () => {
              onSubmitChannel(input)
              setInput('')
            }}
          >
            Add
          </button>
          </div>
        </form>
      
    </div>
  )
}