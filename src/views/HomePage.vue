<template>
  <div class="home-container" :class="{ 'dark': isDark }">
    <div class="content">
      <div class="hero-section">
        <div class="floating-hearts">
          <div class="heart heart-1">💕</div>
          <div class="heart heart-2">💖</div>
          <div class="heart heart-3">💗</div>
          <div class="heart heart-4">💝</div>
        </div>
        
        <h1 class="title">
          <span class="title-line">Um Momento</span>
          <span class="title-line gradient-text">Especial</span>
        </h1>
        
        <p class="subtitle">
          Descubra o tempo que se passou desde um momento único
        </p>
        
        <button 
          class="main-button"
          @click="goToCountdown"
          @mouseover="playHoverAnimation"
        >
          <span class="button-text">Clique aqui</span>
          <div class="button-sparkles">
            <div class="sparkle sparkle-1">✨</div>
            <div class="sparkle sparkle-2">⭐</div>
            <div class="sparkle sparkle-3">💫</div>
          </div>
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'
import { useTheme } from '../composables/useTheme'

const router = useRouter()
const { isDark } = useTheme()

const goToCountdown = () => {
  router.push('/countdown')
}

const playHoverAnimation = () => {
  const button = document.querySelector('.main-button')
  if (button) {
    button.classList.add('pulse')
    setTimeout(() => {
      button.classList.remove('pulse')
    }, 300)
  }
}
</script>

<style scoped>
.home-container {
  min-height: 100vh;
  background: linear-gradient(135deg, #FFF0F5 0%, #E6E6FA 50%, #FFC0CB 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 2rem;
  position: relative;
  overflow: hidden;
  transition: all 0.3s ease;
}

.home-container.dark {
  background: linear-gradient(135deg, #1F2937 0%, #374151 50%, #4B5563 100%);
}

.content {
  text-align: center;
  max-width: 600px;
  position: relative;
  z-index: 2;
}

.floating-hearts {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  pointer-events: none;
  z-index: 1;
}

.heart {
  position: absolute;
  font-size: 2rem;
  opacity: 0.3;
  animation: float 6s ease-in-out infinite;
}

.heart-1 {
  top: 10%;
  left: 10%;
  animation-delay: 0s;
}

.heart-2 {
  top: 20%;
  right: 15%;
  animation-delay: 2s;
}

.heart-3 {
  bottom: 20%;
  left: 20%;
  animation-delay: 4s;
}

.heart-4 {
  bottom: 10%;
  right: 10%;
  animation-delay: 1s;
}

.hero-section {
  position: relative;
  padding: 3rem 2rem;
  background: var(--hero-bg);
  backdrop-filter: blur(10px);
  border-radius: 30px;
  box-shadow: var(--hero-shadow);
  border: 1px solid var(--hero-border);
  transition: all 0.3s ease;
}

.title {
  font-size: 3.5rem;
  font-weight: 700;
  margin-bottom: 1rem;
  line-height: 1.2;
}

.title-line {
  display: block;
  color: var(--title-color);
}

.gradient-text {
  background: linear-gradient(45deg, #EC4899, #8B5CF6);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.subtitle {
  font-size: 1.25rem;
  color: var(--subtitle-color);
  margin-bottom: 3rem;
  line-height: 1.6;
}

.main-button {
  position: relative;
  background: linear-gradient(135deg, #EC4899, #8B5CF6);
  color: white;
  border: none;
  padding: 1.5rem 3rem;
  font-size: 1.2rem;
  font-weight: 600;
  border-radius: 50px;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 10px 25px rgba(236, 72, 153, 0.3);
  text-transform: uppercase;
  letter-spacing: 1px;
  overflow: hidden;
}

.main-button:hover {
  transform: translateY(-3px);
  box-shadow: 0 15px 35px rgba(236, 72, 153, 0.4);
}

.main-button:active {
  transform: translateY(-1px);
}

.main-button.pulse {
  animation: pulse 0.3s ease;
}

.button-text {
  position: relative;
  z-index: 2;
}

.button-sparkles {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  pointer-events: none;
}

.sparkle {
  position: absolute;
  opacity: 0;
  animation: sparkle 2s ease-in-out infinite;
}

.sparkle-1 {
  top: 20%;
  left: 20%;
  animation-delay: 0s;
}

.sparkle-2 {
  top: 60%;
  right: 25%;
  animation-delay: 0.7s;
}

.sparkle-3 {
  bottom: 25%;
  left: 60%;
  animation-delay: 1.4s;
}

@keyframes float {
  0%, 100% {
    transform: translateY(0px) rotate(0deg);
  }
  50% {
    transform: translateY(-20px) rotate(180deg);
  }
}

@keyframes pulse {
  0% {
    transform: scale(1);
  }
  50% {
    transform: scale(1.05);
  }
  100% {
    transform: scale(1);
  }
}

@keyframes sparkle {
  0%, 100% {
    opacity: 0;
    transform: scale(0);
  }
  50% {
    opacity: 1;
    transform: scale(1);
  }
}

/* Variáveis CSS para temas */
:root {
  --hero-bg: rgba(255, 255, 255, 0.1);
  --hero-shadow: 0 20px 40px rgba(0, 0, 0, 0.1);
  --hero-border: rgba(255, 255, 255, 0.2);
  --title-color: #6B46C1;
  --subtitle-color: #6B7280;
}

.dark .hero-section {
  --hero-bg: rgba(0, 0, 0, 0.2);
  --hero-shadow: 0 20px 40px rgba(0, 0, 0, 0.3);
  --hero-border: rgba(255, 255, 255, 0.1);
  --title-color: #E5E7EB;
  --subtitle-color: #D1D5DB;
}

/* Responsive Design */
@media (max-width: 768px) {
  .home-container {
    padding: 1rem;
  }
  
  .title {
    font-size: 2.5rem;
  }
  
  .subtitle {
    font-size: 1.1rem;
  }
  
  .main-button {
    padding: 1.25rem 2.5rem;
    font-size: 1.1rem;
  }
  
  .hero-section {
    padding: 2rem 1.5rem;
  }
}

@media (max-width: 480px) {
  .title {
    font-size: 2rem;
  }
  
  .subtitle {
    font-size: 1rem;
  }
  
  .main-button {
    padding: 1rem 2rem;
    font-size: 1rem;
  }
}
</style>