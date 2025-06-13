<template>
  <div class="countdown-container" :class="{ 'dark': isDark }">
    <div class="background-animation">
      <div class="floating-element element-1">💕</div>
      <div class="floating-element element-2">💖</div>
      <div class="floating-element element-3">💗</div>
      <div class="floating-element element-4">💝</div>
      <div class="floating-element element-5">💘</div>
    </div>
    
    <div class="content">
      <header class="header">
        <button class="back-button" @click="goBack">
          ← Voltar
        </button>
        <h1 class="page-title">
          <span class="title-gradient">Nosso Tempo Especial</span>
        </h1>
        <p class="page-subtitle">
          Desde 21 de Março de 2022, 10:30:00
        </p>
      </header>

      <div class="countdown-section">
        <div class="countdown-grid">
          <div class="time-unit">
            <div class="time-value">{{ countdown.years }}</div>
            <div class="time-label">{{ countdown.years === 1 ? 'Ano' : 'Anos' }}</div>
          </div>
          
          <div class="time-unit">
            <div class="time-value">{{ countdown.months }}</div>
            <div class="time-label">{{ countdown.months === 1 ? 'Mês' : 'Meses' }}</div>
          </div>
          
          <div class="time-unit">
            <div class="time-value">{{ countdown.days }}</div>
            <div class="time-label">{{ countdown.days === 1 ? 'Dia' : 'Dias' }}</div>
          </div>
          
          <div class="time-unit">
            <div class="time-value">{{ countdown.hours }}</div>
            <div class="time-label">{{ countdown.hours === 1 ? 'Hora' : 'Horas' }}</div>
          </div>
          
          <div class="time-unit">
            <div class="time-value">{{ countdown.minutes }}</div>
            <div class="time-label">{{ countdown.minutes === 1 ? 'Minuto' : 'Minutos' }}</div>
          </div>
          
          <div class="time-unit">
            <div class="time-value">{{ countdown.seconds }}</div>
            <div class="time-label">{{ countdown.seconds === 1 ? 'Segundo' : 'Segundos' }}</div>
          </div>
          
          <div class="time-unit milliseconds">
            <div class="time-value">{{ countdown.milliseconds.toString().padStart(3, '0') }}</div>
            <div class="time-label">Milissegundos</div>
          </div>
        </div>
        
        <div class="total-time">
          <div class="total-item">
            <span class="total-value">{{ formatNumber(totalStats.totalDays) }}</span>
            <span class="total-label">dias totais</span>
          </div>
          <div class="total-item">
            <span class="total-value">{{ formatNumber(totalStats.totalHours) }}</span>
            <span class="total-label">horas totais</span>
          </div>
          <div class="total-item">
            <span class="total-value">{{ formatNumber(totalStats.totalMinutes) }}</span>
            <span class="total-label">minutos totais</span>
          </div>
        </div>
      </div>

      <!-- Galeria de Fotos -->
      <PhotoGallery />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { useTheme } from '../composables/useTheme'
import PhotoGallery from '../components/PhotoGallery.vue'

const router = useRouter()
const { isDark } = useTheme()

interface CountdownData {
  years: number
  months: number
  days: number
  hours: number
  minutes: number
  seconds: number
  milliseconds: number
}

interface TotalStats {
  totalDays: number
  totalHours: number
  totalMinutes: number
  totalSeconds: number
  totalMilliseconds: number
}

const countdown = ref<CountdownData>({
  years: 0,
  months: 0,
  days: 0,
  hours: 0,
  minutes: 0,
  seconds: 0,
  milliseconds: 0
})

const totalStats = ref<TotalStats>({
  totalDays: 0,
  totalHours: 0,
  totalMinutes: 0,
  totalSeconds: 0,
  totalMilliseconds: 0
})

let intervalId: number | null = null

const startDate = new Date('2022-03-21T10:30:00.000Z')

const updateCountdown = () => {
  const now = new Date()
  const timeDiff = now.getTime() - startDate.getTime()
  
  const years = Math.floor(timeDiff / (365.25 * 24 * 60 * 60 * 1000))
  const months = Math.floor((timeDiff % (365.25 * 24 * 60 * 60 * 1000)) / (30.44 * 24 * 60 * 60 * 1000))
  const days = Math.floor((timeDiff % (30.44 * 24 * 60 * 60 * 1000)) / (24 * 60 * 60 * 1000))
  const hours = Math.floor((timeDiff % (24 * 60 * 60 * 1000)) / (60 * 60 * 1000))
  const minutes = Math.floor((timeDiff % (60 * 60 * 1000)) / (60 * 1000))
  const seconds = Math.floor((timeDiff % (60 * 1000)) / 1000)
  const milliseconds = timeDiff % 1000
  
  countdown.value = {
    years,
    months,
    days,
    hours,
    minutes,
    seconds,
    milliseconds
  }
  
  totalStats.value = {
    totalDays: Math.floor(timeDiff / (24 * 60 * 60 * 1000)),
    totalHours: Math.floor(timeDiff / (60 * 60 * 1000)),
    totalMinutes: Math.floor(timeDiff / (60 * 1000)),
    totalSeconds: Math.floor(timeDiff / 1000),
    totalMilliseconds: timeDiff
  }
}

const formatNumber = (num: number): string => {
  return num.toLocaleString('pt-BR')
}

const goBack = () => {
  router.push('/')
}

onMounted(() => {
  updateCountdown()
  intervalId = setInterval(updateCountdown, 1)
})

onUnmounted(() => {
  if (intervalId) {
    clearInterval(intervalId)
  }
})
</script>

<style scoped>
.countdown-container {
  min-height: 100vh;
  background: linear-gradient(135deg, #FFF0F5 0%, #E6E6FA 30%, #A9A9E1 70%, #FFC0CB 100%);
  position: relative;
  overflow: hidden;
  transition: all 0.3s ease;
}

.countdown-container.dark {
  background: linear-gradient(135deg, #111827 0%, #1F2937 30%, #374151 70%, #4B5563 100%);
}

.background-animation {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  pointer-events: none;
  z-index: 1;
}

.floating-element {
  position: absolute;
  font-size: 1.5rem;
  opacity: 0.2;
  animation: floatAround 12s ease-in-out infinite;
}

.element-1 {
  top: 10%;
  left: 5%;
  animation-delay: 0s;
}

.element-2 {
  top: 20%;
  right: 10%;
  animation-delay: 2.4s;
}

.element-3 {
  bottom: 30%;
  left: 8%;
  animation-delay: 4.8s;
}

.element-4 {
  bottom: 15%;
  right: 5%;
  animation-delay: 7.2s;
}

.element-5 {
  top: 50%;
  left: 50%;
  animation-delay: 9.6s;
}

.content {
  position: relative;
  z-index: 2;
  padding: 2rem;
  max-width: 1200px;
  margin: 0 auto;
}

.header {
  text-align: center;
  margin-bottom: 3rem;
}

.back-button {
  position: absolute;
  top: 2rem;
  left: 2rem;
  background: var(--button-bg);
  border: none;
  padding: 0.75rem 1.5rem;
  border-radius: 25px;
  cursor: pointer;
  font-weight: 600;
  color: var(--button-color);
  transition: all 0.3s ease;
  backdrop-filter: blur(10px);
  box-shadow: var(--button-shadow);
}

.back-button:hover {
  transform: translateY(-2px);
  box-shadow: var(--button-hover-shadow);
}

.page-title {
  font-size: 3rem;
  font-weight: 700;
  margin-bottom: 1rem;
}

.title-gradient {
  background: linear-gradient(45deg, #EC4899, #8B5CF6);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.page-subtitle {
  font-size: 1.2rem;
  color: var(--subtitle-color);
  margin-bottom: 2rem;
}

.countdown-section {
  background: var(--section-bg);
  backdrop-filter: blur(15px);
  border-radius: 30px;
  padding: 3rem;
  margin-bottom: 3rem;
  box-shadow: var(--section-shadow);
  border: 1px solid var(--section-border);
}

.countdown-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(140px, 1fr));
  gap: 2rem;
  margin-bottom: 3rem;
}

.time-unit {
  text-align: center;
  background: var(--unit-bg);
  border-radius: 20px;
  padding: 2rem 1rem;
  backdrop-filter: blur(10px);
  border: 1px solid var(--unit-border);
  transition: transform 0.3s ease;
}

.time-unit:hover {
  transform: translateY(-5px);
}

.time-unit.milliseconds {
  background: linear-gradient(135deg, #EC4899, #8B5CF6);
  color: white;
}

.time-value {
  font-size: 2.5rem;
  font-weight: 700;
  line-height: 1;
  margin-bottom: 0.5rem;
  color: var(--time-value-color);
}

.time-unit.milliseconds .time-value {
  color: white;
}

.time-label {
  font-size: 0.9rem;
  font-weight: 500;
  text-transform: uppercase;
  letter-spacing: 1px;
  opacity: 0.8;
  color: var(--time-label-color);
}

.time-unit.milliseconds .time-label {
  color: white;
}

.total-time {
  display: flex;
  justify-content: center;
  gap: 3rem;
  flex-wrap: wrap;
  padding-top: 2rem;
  border-top: 1px solid var(--border-color);
}

.total-item {
  text-align: center;
}

.total-value {
  display: block;
  font-size: 1.5rem;
  font-weight: 700;
  color: #EC4899;
  margin-bottom: 0.25rem;
}

.total-label {
  font-size: 0.85rem;
  color: var(--total-label-color);
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

@keyframes floatAround {
  0%, 100% {
    transform: translate(0, 0) rotate(0deg);
  }
  25% {
    transform: translate(30px, -30px) rotate(90deg);
  }
  50% {
    transform: translate(-20px, -60px) rotate(180deg);
  }
  75% {
    transform: translate(-40px, -20px) rotate(270deg);
  }
}

/* Variáveis CSS para temas */
:root {
  --button-bg: rgba(255, 255, 255, 0.9);
  --button-color: #6B46C1;
  --button-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
  --button-hover-shadow: 0 6px 20px rgba(0, 0, 0, 0.15);
  --subtitle-color: #6B7280;
  --section-bg: rgba(255, 255, 255, 0.1);
  --section-shadow: 0 20px 40px rgba(0, 0, 0, 0.1);
  --section-border: rgba(255, 255, 255, 0.2);
  --unit-bg: rgba(255, 255, 255, 0.2);
  --unit-border: rgba(255, 255, 255, 0.3);
  --time-value-color: #374151;
  --time-label-color: #6B7280;
  --border-color: rgba(255, 255, 255, 0.3);
  --total-label-color: #6B7280;
}

.dark {
  --button-bg: rgba(0, 0, 0, 0.3);
  --button-color: #E5E7EB;
  --button-shadow: 0 4px 15px rgba(0, 0, 0, 0.3);
  --button-hover-shadow: 0 6px 20px rgba(0, 0, 0, 0.4);
  --subtitle-color: #D1D5DB;
  --section-bg: rgba(0, 0, 0, 0.2);
  --section-shadow: 0 20px 40px rgba(0, 0, 0, 0.3);
  --section-border: rgba(255, 255, 255, 0.1);
  --unit-bg: rgba(0, 0, 0, 0.2);
  --unit-border: rgba(255, 255, 255, 0.1);
  --time-value-color: #F9FAFB;
  --time-label-color: #D1D5DB;
  --border-color: rgba(255, 255, 255, 0.1);
  --total-label-color: #D1D5DB;
}

/* Responsive Design */
@media (max-width: 768px) {
  .content {
    padding: 1rem;
  }
  
  .back-button {
    position: relative;
    top: 0;
    left: 0;
    margin-bottom: 2rem;
  }
  
  .page-title {
    font-size: 2.5rem;
  }
  
  .countdown-section {
    padding: 2rem;
  }
  
  .countdown-grid {
    grid-template-columns: repeat(auto-fit, minmax(120px, 1fr));
    gap: 1.5rem;
  }
  
  .time-unit {
    padding: 1.5rem 0.75rem;
  }
  
  .time-value {
    font-size: 2rem;
  }
  
  .total-time {
    gap: 2rem;
  }
}

@media (max-width: 480px) {
  .page-title {
    font-size: 2rem;
  }
  
  .countdown-grid {
    grid-template-columns: repeat(2, 1fr);
  }
  
  .time-value {
    font-size: 1.75rem;
  }
  
  .total-time {
    flex-direction: column;
    gap: 1rem;
  }
}
</style>