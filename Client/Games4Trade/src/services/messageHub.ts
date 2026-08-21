import {
  HubConnectionBuilder,
  HubConnectionState,
  type HubConnection
} from '@microsoft/signalr'

export interface Message {
  id: number
  content: string
  isDelivered: boolean
  dateCreated: string
  senderId: number
  receiverId: number
}

type MessageListener = (message: Message) => void

interface ActiveConnection {
  hub: HubConnection
  identity: string
}

const messageListeners = new Set<MessageListener>()
let activeConnection: ActiveConnection | null = null
let connectionTransition: Promise<void> = Promise.resolve()

function runConnectionTransition (transition: () => Promise<void>): Promise<void> {
  connectionTransition = connectionTransition.then(transition, transition)
  return connectionTransition
}

async function stopActiveConnection (): Promise<void> {
  const connectionToStop = activeConnection
  activeConnection = null

  if (!connectionToStop) {
    return
  }

  connectionToStop.hub.off('Recieve')
  if (connectionToStop.hub.state !== HubConnectionState.Disconnected) {
    await connectionToStop.hub.stop()
  }
}

export function subscribeToMessages (listener: MessageListener): () => void {
  messageListeners.add(listener)
  return () => {
    messageListeners.delete(listener)
  }
}

export function connectToMessageHub (url: string, accessToken: string): Promise<void> {
  return runConnectionTransition(async () => {
    const identity = `${url}\u0000${accessToken}`
    if (activeConnection?.identity === identity &&
        activeConnection.hub.state !== HubConnectionState.Disconnected) {
      return
    }

    await stopActiveConnection()

    const hub = new HubConnectionBuilder()
      .withUrl(url, {
        accessTokenFactory: () => accessToken
      })
      .withAutomaticReconnect()
      .build()

    hub.on('Recieve', (message: Message) => {
      for (const listener of messageListeners) {
        listener(message)
      }
    })

    activeConnection = { hub, identity }

    try {
      await hub.start()
    } catch (error) {
      if (activeConnection?.hub === hub) {
        activeConnection = null
      }
      hub.off('Recieve')
      throw error
    }
  })
}

export function disconnectFromMessageHub (): Promise<void> {
  return runConnectionTransition(stopActiveConnection)
}
