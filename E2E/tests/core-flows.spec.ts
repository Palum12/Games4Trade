import { expect, test, type Page } from '@playwright/test'

// End-to-end scenarios for the application running through Docker Compose.

const runId = `${Date.now()}${Math.floor(Math.random() * 10_000)}`
const genreName = `E2E gatunek ${runId}`
const systemManufacturer = `E2E producent ${runId}`
const systemModel = `E2E model ${runId}`

test.describe.configure({ mode: 'serial' })

async function confirmDialog(page: Page) {
  const confirmButton = page.locator('.swal2-confirm')
  await expect(confirmButton).toBeVisible()
  await expect(page.locator('.swal2-container')).toHaveCSS('position', 'fixed')
  await confirmButton.click()
}

async function confirmSuccessDialog(page: Page) {
  await expect(page.getByRole('heading', { name: 'Akcja zakończona sukcesem !' })).toBeVisible()
  await confirmDialog(page)
}

test('administrator can sign in and add a genre and system', async ({ page }) => {
  await page.goto('/login')
  await page.locator('#login').fill('admin')
  await page.locator('#password').fill('Admin123!')
  await page.getByRole('button', { name: 'Zaloguj' }).click()

  await expect(page.getByRole('link', { name: 'Panel administratora' })).toBeVisible()
  await expect(page.getByRole('button', { name: 'Wyloguj' })).toHaveCSS('padding-top', '8px')
  await page.getByRole('link', { name: 'Panel administratora' }).click()

  await page.getByRole('button', { name: 'Dodaj nowy gatunek' }).click()
  await page.locator('.genres input').last().fill(genreName)
  await page.locator('.genres').getByRole('button', { name: 'Zapisz' }).click()
  await confirmDialog(page)
  await confirmSuccessDialog(page)
  await expect(page.locator('.genres input').last()).toHaveValue(genreName)

  await page.getByRole('button', { name: 'Dodaj nowy system' }).click()
  const newSystemRow = page.locator('.systems .form-row').last()
  await newSystemRow.locator('#Manufacturer').fill(systemManufacturer)
  await newSystemRow.locator('#Model').fill(systemModel)
  await page.locator('.systems').getByRole('button', { name: 'Zapisz' }).click()
  await confirmDialog(page)
  await confirmSuccessDialog(page)
  await expect(page.locator('.systems .form-row').last().locator('#Manufacturer')).toHaveValue(systemManufacturer)
})

test('visitor can create an account, sign in and add an advertisement', async ({ page }) => {
  const login = `e2euser${runId}`
  const email = `${login}@example.test`
  const advertisementTitle = `Ogłoszenie E2E ${runId}`

  await page.goto('/signup')
  await page.locator('#email').fill(email)
  await page.locator('#login').fill(login)
  await page.locator('#login').press('Tab')
  await page.locator('#acceptTerms').check()
  await expect(page.getByRole('button', { name: 'Utwórz konto !' })).toBeEnabled()
  await page.getByRole('button', { name: 'Utwórz konto !' }).click()
  await expect(page.locator('.swal2-popup')).toBeVisible()
  await confirmDialog(page)

  await page.goto('/login')
  await page.locator('#login').fill(login)
  await page.locator('#password').fill('TempPass')
  await page.getByRole('button', { name: 'Zaloguj' }).click()
  await expect(page.getByRole('link', { name: 'Dodaj ogłoszenie' })).toBeVisible()

  await page.getByRole('link', { name: 'Dodaj ogłoszenie' }).click()
  const addButton = page.getByRole('button', { name: 'Dodaj ogłoszenie!' })
  await addButton.click()
  await expect(page.locator('#title')).toHaveClass(/is-invalid/)
  await expect(page.locator('#dateReleased')).toHaveClass(/is-invalid/)
  await expect(page.locator('#state')).toHaveClass(/is-invalid/)
  await expect(page.locator('#genre')).toHaveClass(/is-invalid/)
  await expect(page.getByText('Proszę podać tytuł ogłoszenia')).toBeVisible()
  await expect(page.getByText('Proszę wybrać gatunek')).toBeVisible()

  await page.locator('#title').fill(advertisementTitle)
  await page.locator('#dateReleased').fill('2020-01-01')
  await page.locator('#price').fill('199.99')
  await page.locator('#developer').fill('Studio E2E')
  await page.locator('#state').selectOption({ index: 1 })
  await page.locator('#system').selectOption({ label: `${systemManufacturer} ${systemModel}` })
  await page.locator('#region').selectOption({ label: 'PAL' })
  await page.locator('#genre').selectOption({ label: genreName })
  await page.locator('#description').fill('Ogłoszenie utworzone automatycznie przez test E2E.')
  await page.locator('input[type="file"]').setInputFiles('../Client/Games4Trade/src/assets/logo.png')
  const photoPreview = page.getByTestId('photo-preview')
  await expect(photoPreview).toBeVisible()
  await expect.poll(() => photoPreview.evaluate((image: HTMLImageElement) => image.complete && image.naturalWidth > 0)).toBe(true)

  await expect(addButton).toBeEnabled()
  await addButton.click()
  await confirmDialog(page)
  await expect(page).toHaveURL(/\/advertisements\/\d+$/)
  await expect(page.locator('.swal2-popup')).toBeVisible()
  await confirmDialog(page)
  await expect(page.getByText(advertisementTitle)).toBeVisible()
  const advertisementUrl = page.url()

  const uploadedImage = page.locator('.gallery img')
  await expect(uploadedImage).toBeVisible()
  await expect.poll(() => uploadedImage.evaluate((image: HTMLImageElement) => image.complete && image.naturalWidth > 0)).toBe(true)

  await page.getByRole('link', { name: 'Games4Trade' }).click()
  await expect(page).toHaveURL(/\/$/)
  await expect(page.locator('h1')).toContainText('Games4Trade')
  await expect(page.getByText(advertisementTitle)).toBeVisible()

  await page.goto(advertisementUrl)
  await page.getByRole('button', { name: 'Modyfikuj' }).click()
  const savedPhotoPreview = page.getByTestId('photo-preview')
  await expect(savedPhotoPreview).toBeVisible()
  await expect.poll(() => savedPhotoPreview.evaluate((image: HTMLImageElement) => image.complete && image.naturalWidth > 0)).toBe(true)
})
