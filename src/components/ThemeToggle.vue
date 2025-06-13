<template>
  <button 
    class="theme-toggle"
    @click="toggleTheme"
    :title="isDark ? 'Mudar para tema claro' : 'Mudar para tema escuro'"
  >
    <div class="toggle-container">
      <div class="toggle-slider" :class="{ 'dark': isDark }">
        <div class="toggle-icon">
          <span v-if="isDark" class="icon">🌙</span>
          <span v-else class="icon">☀️</span>
        </div>
      </div>
    </div>
  </button>
</template>

<script setup lang="ts">
import { useTheme } from '../composables/useTheme'

const { isDark, toggleTheme } = useTheme()
</script>

<style scoped>
.theme-toggle {
  position: fixed;
  top: 2rem;
  right: 2rem;
  z-index: 1000;
  background: none;
  border: none;
  cursor: pointer;
  padding: 0;
}

.toggle-container {
  width: 60px;
  height: 30px;
  background: var(--toggle-bg);
  border-radius: 15px;
  position: relative;
  transition: all 0.3s ease;
  border: 2px solid var(--toggle-border);
  box-shadow: 0 4px 15px var(--toggle-shadow);
}

.toggle-slider {
  width: 26px;
  height: 26px;
  background: var(--toggle-slider-bg);
  border-radius: 50%;
  position: absolute;
  top: 50%;
  left: 2px;
  transform: translateY(-50%);
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 2px 8px var(--toggle-slider-shadow);
}

.toggle-slider.dark {
  left: calc(100% - 28px);
}

.toggle-icon {
  display: flex;
  align-items: center;
  justify-content: center;
}

.icon {
  font-size: 0.9rem;
  transition: all 0.3s ease;
}

.theme-toggle:hover .toggle-container {
  transform: scale(1.05);
}

/* Variáveis CSS para temas */
:root {
  --toggle-bg: rgba(255, 255, 255, 0.2);
  --toggle-border: rgba(255, 255, 255, 0.3);
  --toggle-shadow: rgba(0, 0, 0, 0.1);
  --toggle-slider-bg: #ffffff;
  --toggle-slider-shadow: rgba(0, 0, 0, 0.2);
}

:global(.dark-theme) {
  --toggle-bg: rgba(0, 0, 0, 0.3);
  --toggle-border: rgba(255, 255, 255, 0.2);
  --toggle-shadow: rgba(0, 0, 0, 0.3);
  --toggle-slider-bg: #2D3748;
  --toggle-slider-shadow: rgba(0, 0, 0, 0.4);
}

@media (max-width: 768px) {
  .theme-toggle {
    top: 1rem;
    right: 1rem;
  }
  
  .toggle-container {
    width: 50px;
    height: 25px;
  }
  
  .toggle-slider {
    width: 21px;
    height: 21px;
  }
  
  .toggle-slider.dark {
    left: calc(100% - 23px);
  }
  
  .icon {
    font-size: 0.8rem;
  }
}
</style>