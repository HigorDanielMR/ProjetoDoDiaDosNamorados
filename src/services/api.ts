import axios from 'axios'

const API_BASE_URL = 'http://localhost:5000/api'

const api = axios.create({
  baseURL: API_BASE_URL,
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
  },
})

export interface CountdownResponse {
  years: number
  months: number
  days: number
  hours: number
  minutes: number
  seconds: number
  milliseconds: number
  totalDays: number
  totalHours: number
  totalMinutes: number
  totalSeconds: number
  totalMilliseconds: number
}

export const countdownApi = {
  getStartDate: async () => {
    try {
      const response = await api.get('/countdown/start-date')
      return response.data
    } catch (error) {
      console.error('Erro ao buscar data inicial:', error)
      throw error
    }
  },

  getCurrentTime: async () => {
    try {
      const response = await api.get('/countdown/current-time')
      return response.data
    } catch (error) {
      console.error('Erro ao buscar tempo atual:', error)
      throw error
    }
  },

  calculateCountdown: async (): Promise<CountdownResponse> => {
    try {
      const response = await api.get('/countdown/calculate')
      return response.data
    } catch (error) {
      console.error('Erro ao calcular contagem:', error)
      throw error
    }
  },
}

export default api