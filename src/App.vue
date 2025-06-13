<template>
  <div id="app">
    <ThemeToggle />
    <router-view />
  </div>
</template>

<script setup lang="ts">
import ThemeToggle from './components/ThemeToggle.vue'
import { useTheme } from './composables/useTheme'

// Inicializa o tema
const { theme } = useTheme()
</script>

<style>
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

body {
  font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, sans-serif;
  line-height: 1.6;
  transition: all 0.3s ease;
}

/* Tema Claro */
.light-theme {
  color: #374151;
  background: #FFFFFF;
}

/* Tema Escuro */
.dark-theme {
  color: #F9FAFB;
  background: #111827;
}

#app {
  width: 100%;
  min-height: 100vh;
}

/* Scrollbar personalizada */
::-webkit-scrollbar {
  width: 8px;
}

::-webkit-scrollbar-track {
  background: var(--scrollbar-track);
}

::-webkit-scrollbar-thumb {
  background: linear-gradient(135deg, #EC4899, #8B5CF6);
  border-radius: 4px;
}

::-webkit-scrollbar-thumb:hover {
  background: linear-gradient(135deg, #DB2777, #7C3AED);
}

/* Seleção de texto */
::selection {
  background: rgba(236, 72, 153, 0.2);
  color: #EC4899;
}

/* Foco para acessibilidade */
*:focus {
  outline: 2px solid #EC4899;
  outline-offset: 2px;
}

/* Animações globais */
@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.fade-in {
  animation: fadeIn 0.6s ease-out;
}

/* Variáveis CSS para temas */
:root {
  --scrollbar-track: #F3F4F6;
}

.dark-theme {
  --scrollbar-track: #374151;
}
</style>