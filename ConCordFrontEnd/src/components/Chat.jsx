import { useState, useEffect } from 'react'
import useSignalR from '../useSignalR'

import Sidebar from './Sidebar'
import Message from './Message'
import Form from './Form'

export default function Chat({ username }) {
  const connection = useSignalR('/r/chatHub')

  const [channels, setChannels] = useState([])
  const [messages, setMessages] = useState([])
  const [channelSelected, setChannelSelected] = useState(null)

  const handleSelectChannel = (channel) => {
    setChannelSelected(channel)
  }
  
  const handleSubmit = async (message) => {
    await fetch(`/api/channels/${channelSelected.id}/messages`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(message),
    })
  }

  const handleSubmitChannel = async (channelName) => {
    if (!channelName || channelName.length === 0) return

    let chName = channelName.slice(0, 50)

    await fetch('/api/channels', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ name: chName }),
    })
      .then((response) => response.json())
      .then((data) => {
        setChannels((channels) => [...channels, data])
      })
  }

  // get channels
  useEffect(() => {
    const fetchChannels = async () => {
      const res = await fetch('/api/channels')
      const json = await res.json()
      setChannels(json)
      if (json.length > 0) {
        setChannelSelected(json[0])
      }
    }
    fetchChannels()
  }, [])

  // get messages for the selected channel
  useEffect(() => {
    if (!channelSelected) return

    const fetchMessages = async () => {
      const res = await fetch(`/api/channels/${channelSelected.id}/messages`)
      const json = await res.json()
      setMessages(json)
    }
    fetchMessages()
  }, [channelSelected])

  useEffect(() => {
    if (!connection || !channelSelected) return

    // only listen for messages coming from a certain chat room
    connection.invoke('AddToGroup', `${channelSelected.id}`)

    // listen for messages from the server
    connection.on('ReceiveMessage', (message) => {
      setMessages((messages) => [...messages, message])
    })

    return () => {
      connection.invoke('RemoveFromGroup', `${channelSelected.id}`)
      connection.off('ReceiveMessage')
    }
  }, [connection, channelSelected])

  useEffect(() => {
    document.getElementById('chatDiv').scrollTo(0, document.getElementById('chatList').scrollHeight)
  }, [messages])

  return (
    <div className="Chat h-full w-full flex flex-col sm:flex-row">
      <Sidebar 
        channels={channels}
        selectedChannel={channelSelected}
        onSelectChannel={handleSelectChannel}
        onSubmitChannel={handleSubmitChannel}
      />

      <div className="main h-3/4 grow sm:h-full w-full flex flex-col">
        <div 
          id='chatDiv' 
          className="Messages bg-white dark:bg-black grow overflow-y-auto border rounded-md border-slate-500 shadow-md shadow-slate-400 dark:shadow-zinc-600 p-4 mb-4 sm:mt-4 sm:mx-4 "
        >
          <ul id='chatList' className='text-left text-clip'>
            {channelSelected && messages.map((message) => (
              <Message key={message.id} message={message} username={username} />
            ))}
          </ul>
        </div>
        
        {channelSelected && (
          <Form userName={username} channelId={channelSelected.id} onSubmit={handleSubmit} />
        )}
      </div>
    </div>
  )
}