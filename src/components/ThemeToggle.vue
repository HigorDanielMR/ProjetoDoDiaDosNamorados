<template>
  <button 
    class="alternador-tema"
    @click="alternarTema"
    :title="temaEscuro ? 'Mudar para tema claro' : 'Mudar para tema escuro'"
  >
    <div class="container-alternador">
      <div class="deslizador-alternador" :class="{ 'escuro': temaEscuro }">
        <div class="icone-alternador">
          <span v-if="temaEscuro" class="icone">🌙</span>
          <span v-else class="icone">☀️</span>
        </div>
      </div>
    </div>
  </button>
</template>

<script setup lang="ts">
import { useTheme } from '../composables/useTheme'

const { isDark: temaEscuro, toggleTheme: alternarTema } = useTheme()
</script>

<style scoped>
.alternador-tema {
  position: fixed;
  top: 2rem;
  right: 2rem;
  z-index: 1000;
  background: none;
  border: none;
  cursor: pointer;
  padding: 0;
}

.container-alternador {
  width: 60px;
  height: 30px;
  background: var(--fundo-alternador);
  border-radius: 15px;
  position: relative;
  transition: all 0.3s ease;
  border: 2px solid var(--borda-alternador);
  box-shadow: 0 4px 15px var(--sombra-alternador);
}

.deslizador-alternador {
  width: 26px;
  height: 26px;
  background: var(--fundo-deslizador);
  border-radius: 50%;
  position: absolute;
  top: 50%;
  left: 2px;
  transform: translateY(-50%);
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 2px 8px var(--sombra-deslizador);
}

.deslizador-alternador.escuro {
  left: calc(100% - 28px);
}

.icone-alternador {
  display: flex;
  align-items: center;
  justify-content: center;
}

.icone {
  font-size: 0.9rem;
  transition: all 0.3s ease;
}

.alternador-tema:hover .container-alternador {
  transform: scale(1.05);
}

/* Variáveis CSS para temas */
:root {
  --fundo-alternador: rgba(255, 255, 255, 0.2);
  --borda-alternador: rgba(255, 255, 255, 0.3);
  --sombra-alternador: rgba(0, 0, 0, 0.1);
  --fundo-deslizador: #ffffff;
  --sombra-deslizador: rgba(0, 0, 0, 0.2);
}

:global(.dark-theme) {
  --fundo-alternador: rgba(0, 0, 0, 0.3);
  --borda-alternador: rgba(255, 255, 255, 0.2);
  --sombra-alternador: rgba(0, 0, 0, 0.3);
  --fundo-deslizador: #2D3748;
  --sombra-deslizador: rgba(0, 0, 0, 0.4);
}

@media (max-width: 768px) {
  .alternador-tema {
    top: 1rem;
    right: 1rem;
  }
  
  .container-alternador {
    width: 50px;
    height: 25px;
  }
  
  .deslizador-alternador {
    width: 21px;
    height: 21px;
  }
  
  .deslizador-alternador.escuro {
    left: calc(100% - 23px);
  }
  
  .icone {
    font-size: 0.8rem;
  }
}
</style>