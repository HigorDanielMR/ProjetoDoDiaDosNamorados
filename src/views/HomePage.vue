<template>
  <div class="container-inicio" :class="{ 'escuro': temaEscuro }">
    <div class="conteudo">
      <div class="secao-heroi">
        <div class="coracoes-flutuantes">
          <div class="coracao coracao-1">💕</div>
          <div class="coracao coracao-2">💖</div>
          <div class="coracao coracao-3">💗</div>
          <div class="coracao coracao-4">💝</div>
        </div>
        
        <h1 class="titulo">
          <span class="linha-titulo">Um Momento</span>
          <span class="linha-titulo texto-gradiente">Especial</span>
        </h1>
        
        <p class="subtitulo">
          Descubra o tempo que se passou desde um momento único
        </p>
        
        <button 
          class="botao-principal"
          @click="irParaContagem"
          @mouseover="reproduzirAnimacaoHover"
        >
          <span class="texto-botao">Clique aqui</span>
          <div class="brilhos-botao">
            <div class="brilho brilho-1">✨</div>
            <div class="brilho brilho-2">⭐</div>
            <div class="brilho brilho-3">💫</div>
          </div>
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'
import { useTheme } from '../composables/useTheme'

const roteador = useRouter()
const { isDark: temaEscuro } = useTheme()

const irParaContagem = () => {
  roteador.push('/countdown')
}

const reproduzirAnimacaoHover = () => {
  const botao = document.querySelector('.botao-principal')
  if (botao) {
    botao.classList.add('pulsar')
    setTimeout(() => {
      botao.classList.remove('pulsar')
    }, 300)
  }
}
</script>

<style scoped>
.container-inicio {
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

.container-inicio.escuro {
  background: linear-gradient(135deg, #1F2937 0%, #374151 50%, #4B5563 100%);
}

.conteudo {
  text-align: center;
  max-width: 600px;
  position: relative;
  z-index: 2;
}

.coracoes-flutuantes {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  pointer-events: none;
  z-index: 1;
}

.coracao {
  position: absolute;
  font-size: 2rem;
  opacity: 0.3;
  animation: flutuar 6s ease-in-out infinite;
}

.coracao-1 {
  top: 10%;
  left: 10%;
  animation-delay: 0s;
}

.coracao-2 {
  top: 20%;
  right: 15%;
  animation-delay: 2s;
}

.coracao-3 {
  bottom: 20%;
  left: 20%;
  animation-delay: 4s;
}

.coracao-4 {
  bottom: 10%;
  right: 10%;
  animation-delay: 1s;
}

.secao-heroi {
  position: relative;
  padding: 3rem 2rem;
  background: var(--fundo-heroi);
  backdrop-filter: blur(10px);
  border-radius: 30px;
  box-shadow: var(--sombra-heroi);
  border: 1px solid var(--borda-heroi);
  transition: all 0.3s ease;
}

.titulo {
  font-size: 3.5rem;
  font-weight: 700;
  margin-bottom: 1rem;
  line-height: 1.2;
}

.linha-titulo {
  display: block;
  color: var(--cor-titulo);
}

.texto-gradiente {
  background: linear-gradient(45deg, #EC4899, #8B5CF6);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.subtitulo {
  font-size: 1.25rem;
  color: var(--cor-subtitulo);
  margin-bottom: 3rem;
  line-height: 1.6;
}

.botao-principal {
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

.botao-principal:hover {
  transform: translateY(-3px);
  box-shadow: 0 15px 35px rgba(236, 72, 153, 0.4);
}

.botao-principal:active {
  transform: translateY(-1px);
}

.botao-principal.pulsar {
  animation: pulsar 0.3s ease;
}

.texto-botao {
  position: relative;
  z-index: 2;
}

.brilhos-botao {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  pointer-events: none;
}

.brilho {
  position: absolute;
  opacity: 0;
  animation: brilhar 2s ease-in-out infinite;
}

.brilho-1 {
  top: 20%;
  left: 20%;
  animation-delay: 0s;
}

.brilho-2 {
  top: 60%;
  right: 25%;
  animation-delay: 0.7s;
}

.brilho-3 {
  bottom: 25%;
  left: 60%;
  animation-delay: 1.4s;
}

@keyframes flutuar {
  0%, 100% {
    transform: translateY(0px) rotate(0deg);
  }
  50% {
    transform: translateY(-20px) rotate(180deg);
  }
}

@keyframes pulsar {
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

@keyframes brilhar {
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
  --fundo-heroi: rgba(255, 255, 255, 0.1);
  --sombra-heroi: 0 20px 40px rgba(0, 0, 0, 0.1);
  --borda-heroi: rgba(255, 255, 255, 0.2);
  --cor-titulo: #6B46C1;
  --cor-subtitulo: #6B7280;
}

.escuro .secao-heroi {
  --fundo-heroi: rgba(0, 0, 0, 0.2);
  --sombra-heroi: 0 20px 40px rgba(0, 0, 0, 0.3);
  --borda-heroi: rgba(255, 255, 255, 0.1);
  --cor-titulo: #E5E7EB;
  --cor-subtitulo: #D1D5DB;
}

/* Design Responsivo */
@media (max-width: 768px) {
  .container-inicio {
    padding: 1rem;
  }
  
  .titulo {
    font-size: 2.5rem;
  }
  
  .subtitulo {
    font-size: 1.1rem;
  }
  
  .botao-principal {
    padding: 1.25rem 2.5rem;
    font-size: 1.1rem;
  }
  
  .secao-heroi {
    padding: 2rem 1.5rem;
  }
}

@media (max-width: 480px) {
  .titulo {
    font-size: 2rem;
  }
  
  .subtitulo {
    font-size: 1rem;
  }
  
  .botao-principal {
    padding: 1rem 2rem;
    font-size: 1rem;
  }
}
</style>