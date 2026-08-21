import { expect, test, type APIRequestContext, type Page } from '@playwright/test'

const runId = `${Date.now()}${Math.floor(Math.random() * 10_000)}`
const apiUrl = process.env.E2E_API_URL ?? 'http://localhost:5001/api'

interface AuthenticatedUser {
  id: number
  login: string
  token: string
}

async function confirmDialog (page: Page): Promise<void> {
  const confirmButton = page.locator('.swal2-confirm')
  await expect(confirmButton).toBeVisible()
  await confirmButton.click()
}

async function registerUser (page: Page, login: string): Promise<void> {
  await page.goto('/signup')
  await page.locator('#email').fill(`${login}@example.test`)
  await page.locator('#login').fill(login)
  await page.locator('#login').press('Tab')
  await page.locator('#acceptTerms').check()

  const registerButton = page.getByRole('button', { name: 'Utwórz konto !' })
  await expect(registerButton).toBeEnabled()
  await registerButton.click()
  await expect(page.locator('.swal2-popup')).toBeVisible()
  await confirmDialog(page)
}

async function loginUser (
  page: Page,
  request: APIRequestContext,
  login: string
): Promise<AuthenticatedUser> {
  await page.goto('/login')

  const hubSocketPromise = page.waitForEvent('websocket', {
    predicate: socket => socket.url().includes('/messagehub')
  })

  await page.locator('#login').fill(login)
  await page.locator('#password').fill('TempPass')
  await page.getByRole('button', { name: 'Zaloguj' }).click()

  await expect(page.getByRole('link', { name: 'Wiadomości' })).toBeVisible()
  const hubSocket = await hubSocketPromise
  expect(hubSocket.url()).toContain('localhost:5001/messagehub')

  const token = await page.evaluate(() => localStorage.getItem('token'))
  expect(token).not.toBeNull()

  const idResponse = await request.get(`${apiUrl}/users/id`, {
    headers: { Authorization: `Bearer ${token}` }
  })
  expect(idResponse.ok()).toBeTruthy()

  return {
    id: Number(await idResponse.text()),
    login,
    token: token as string
  }
}

async function openConversation (page: Page, otherUserLogin: string): Promise<void> {
  await page.getByRole('link', { name: 'Wiadomości' }).click()
  const conversationMiniature = page.getByText(otherUserLogin, { exact: true })
  await expect(conversationMiniature).toBeVisible()
  await conversationMiniature.click()
  await expect(page.locator('textarea')).toBeVisible()
}

test('two signed-in users exchange messages through SignalR without polling', async ({ browser, request }) => {
  const senderLogin = `e2esender${runId}`
  const receiverLogin = `e2ereceiver${runId}`
  const senderContext = await browser.newContext()
  const receiverContext = await browser.newContext()
  const senderPage = await senderContext.newPage()
  const receiverPage = await receiverContext.newPage()

  try {
    await registerUser(senderPage, senderLogin)
    await registerUser(receiverPage, receiverLogin)

    const sender = await loginUser(senderPage, request, senderLogin)
    const receiver = await loginUser(receiverPage, request, receiverLogin)

    const bootstrapResponse = await request.post(`${apiUrl}/messages`, {
      headers: { Authorization: `Bearer ${sender.token}` },
      data: {
        receiverId: receiver.id,
        content: `Rozpoczęcie rozmowy ${runId}`
      }
    })
    expect(bootstrapResponse.ok()).toBeTruthy()

    const disablePolling = /\/api\/Messages\/\d+\/isUpdate(?:\?|$)/i
    await senderContext.route(disablePolling, route => route.fulfill({
      status: 200,
      contentType: 'application/json',
      body: 'false'
    }))
    await receiverContext.route(disablePolling, route => route.fulfill({
      status: 200,
      contentType: 'application/json',
      body: 'false'
    }))

    await openConversation(senderPage, receiver.login)
    await openConversation(receiverPage, sender.login)

    const messageToReceiver = `Wiadomość SignalR do odbiorcy ${runId}`
    await senderPage.locator('textarea').fill(messageToReceiver)
    await senderPage.getByRole('button', { name: 'Wyślij' }).click()
    await expect(receiverPage.locator('#inner').getByText(messageToReceiver, { exact: true })).toBeVisible()

    const replyToSender = `Odpowiedź SignalR do nadawcy ${runId}`
    await receiverPage.locator('textarea').fill(replyToSender)
    await receiverPage.getByRole('button', { name: 'Wyślij' }).click()
    await expect(senderPage.locator('#inner').getByText(replyToSender, { exact: true })).toBeVisible()
  } finally {
    await senderContext.close()
    await receiverContext.close()
  }
})
