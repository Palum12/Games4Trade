import { spawn } from 'node:child_process'
import { fileURLToPath } from 'node:url'
import { dirname, resolve } from 'node:path'

const e2eDirectory = dirname(fileURLToPath(import.meta.url))
const composeFile = resolve(e2eDirectory, 'docker-compose.e2e.yml')
const playwrightCli = resolve(e2eDirectory, 'node_modules', '@playwright', 'test', 'cli.js')
const baseUrl = process.env.E2E_BASE_URL ?? 'http://localhost:8080'
const apiReadinessUrl = process.env.E2E_API_READINESS_URL ?? 'http://localhost:5001/api/advertisements'
const composeArguments = ['compose', '--project-name', 'games4trade-e2e', '--file', composeFile]

function run (command, args, options = {}) {
  return new Promise((resolvePromise, reject) => {
    const child = spawn(command, args, {
      cwd: e2eDirectory,
      env: process.env,
      stdio: 'inherit',
      ...options
    })

    child.on('error', reject)
    child.on('exit', code => {
      if (code === 0) {
        resolvePromise()
      } else {
        reject(new Error(`${command} zakończył się kodem ${code}`))
      }
    })
  })
}

function pause (milliseconds) {
  return new Promise(resolvePromise => setTimeout(resolvePromise, milliseconds))
}

async function waitForApplication (url, serviceName, requireSuccessStatus) {
  const timeoutAt = Date.now() + 120_000

  while (Date.now() < timeoutAt) {
    try {
      const response = await fetch(url, { signal: AbortSignal.timeout(2_000) })
      if (!requireSuccessStatus || response.ok) {
        return
      }
    } catch {
      // The frontend or API is still starting up.
    }
    await pause(1_000)
  }

  throw new Error(`${serviceName} E2E nie uruchomił się w ciągu 120 sekund: ${url}`)
}

async function tearDown () {
  await run('docker', [...composeArguments, 'down', '--volumes', '--remove-orphans'])
}

async function prepareApiImage () {
  try {
    await run('docker', ['image', 'inspect', 'games4trade-api:latest'], { stdio: 'ignore' })
  } catch {
    throw new Error('Brak lokalnego obrazu API. W katalogu głównym wykonaj najpierw: docker compose build api')
  }

  await run('docker', ['image', 'tag', 'games4trade-api:latest', 'games4trade-api:e2e'])
}

let testFailed = true

try {
  // A leftover E2E project (for example after a stopped terminal) must not retain its data.
  await tearDown()
  await prepareApiImage()
  await run('docker', [...composeArguments, 'build', 'client'])
  await run('docker', [...composeArguments, 'up', '--detach'])
  await waitForApplication(apiReadinessUrl, 'API', false)
  await waitForApplication(baseUrl, 'Frontend', true)
  await run(process.execPath, [playwrightCli, 'test', ...process.argv.slice(2)], {
    env: { ...process.env, E2E_BASE_URL: baseUrl }
  })
  testFailed = false
} catch (error) {
  console.error(error.message)
} finally {
  try {
    await tearDown()
  } catch (error) {
    console.error(`Nie udało się posprzątać środowiska E2E: ${error.message}`)
    testFailed = true
  }
}

process.exit(testFailed ? 1 : 0)
