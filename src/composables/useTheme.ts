import { ref, computed, watch } from 'vue'

export type Theme = 'light' | 'dark'

const theme = ref<Theme>('light')

// Carrega o tema salvo do localStorage
const savedTheme = localStorage.getItem('theme') as Theme
if (savedTheme) {
  theme.value = savedTheme
}

export const useTheme = () => {
  const isDark = computed(() => theme.value === 'dark')
  
  const toggleTheme = () => {
    theme.value = theme.value === 'light' ? 'dark' : 'light'
  }
  
  const setTheme = (newTheme: Theme) => {
    theme.value = newTheme
  }
  
  // Salva o tema no localStorage e aplica a classe no body
  watch(theme, (newTheme) => {
    localStorage.setItem('theme', newTheme)
    document.body.className = newTheme === 'dark' ? 'dark-theme' : 'light-theme'
  }, { immediate: true })
  
  return {
    theme: computed(() => theme.value),
    isDark,
    toggleTheme,
    setTheme
  }
}