import { useEffect, useState } from 'react'
import {
  HubConnection,
  HubConnectionBuilder,
  LogLevel,
} from '@microsoft/signalr'

export default function useSignalR(url) {
  const [connection, setConnection] = useState(null)

  useEffect(() => {
    // Cancel everything if the component unmounts
    let isCancelled = false

    // Build a connection to the SignalR server. Automatically reconnects if the connection is lost.
    const connection = new HubConnectionBuilder()
      .withUrl(url)
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Information)
      .build()

    // Try to start the connection
    connection
      .start()
      .then(() => {
        if (!isCancelled) {
          setConnection(connection)
        }
      })
      .catch((err) => {
        console.error("SignalR connection error:", err)
      })

    // Handle the connection being closed
    connection.onclose(() => {
      if (!isCancelled) {
        setConnection(null)
      }

      setConnection(null)
    })

    // If the connection is lost, it won't close. Instead, it will try to reconnect.
    // So we need to treat it as a lost connection until 'onreconnected' is called.
    connection.onreconnecting(() => {
      if (!isCancelled) {
        return
      }

      setConnection(null)
    })

    // Handle the connection being reconnected
    connection.onreconnected(() => {
      if (!isCancelled) {
        return
      }

      setConnection(connection)
    })

    // Clean up the connection when the component unmounts
    return () => {
      isCancelled = true
      connection.stop()
    }
  }, [])

  return connection
}