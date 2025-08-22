<template>
  <div class="container-contagem" :class="{ 'dark': temaEscuro }">
    <div class="animacao-fundo">
      <div class="elemento-flutuante elemento-1">💕</div>
      <div class="elemento-flutuante elemento-2">💖</div>
      <div class="elemento-flutuante elemento-3">💗</div>
      <div class="elemento-flutuante elemento-4">💝</div>
      <div class="elemento-flutuante elemento-5">💘</div>
    </div>
    
    <div class="conteudo">
      <header class="cabecalho">
        <button class="botao-voltar" @click="voltarPagina">
          ← Voltar
        </button>
        <h1 class="titulo-pagina">
          <span class="gradiente-titulo">Nosso Tempo Especial</span>
        </h1>
        <p class="subtitulo-pagina">
          Desde 21 de Março de 2022, 00:00:00
        </p>
      </header>

      <div class="secao-contagem">
        <div class="grade-contagem">
          <div class="unidade-tempo">
            <div class="valor-tempo">{{ contagem.anos }}</div>
            <div class="rotulo-tempo">{{ contagem.anos === 1 ? 'Ano' : 'Anos' }}</div>
          </div>
          
          <div class="unidade-tempo">
            <div class="valor-tempo">{{ contagem.meses }}</div>
            <div class="rotulo-tempo">{{ contagem.meses === 1 ? 'Mês' : 'Meses' }}</div>
          </div>
          
          <div class="unidade-tempo">
            <div class="valor-tempo">{{ contagem.dias }}</div>
            <div class="rotulo-tempo">{{ contagem.dias === 1 ? 'Dia' : 'Dias' }}</div>
          </div>
          
          <div class="unidade-tempo">
            <div class="valor-tempo">{{ contagem.horas }}</div>
            <div class="rotulo-tempo">{{ contagem.horas === 1 ? 'Hora' : 'Horas' }}</div>
          </div>
          
          <div class="unidade-tempo">
            <div class="valor-tempo">{{ contagem.minutos }}</div>
            <div class="rotulo-tempo">{{ contagem.minutos === 1 ? 'Minuto' : 'Minutos' }}</div>
          </div>
          
          <div class="unidade-tempo">
            <div class="valor-tempo">{{ contagem.segundos }}</div>
            <div class="rotulo-tempo">{{ contagem.segundos === 1 ? 'Segundo' : 'Segundos' }}</div>
          </div>
          
          <div class="unidade-tempo milissegundos">
            <div class="valor-tempo">{{ contagem.milissegundos.toString().padStart(3, '0') }}</div>
            <div class="rotulo-tempo">Milissegundos</div>
          </div>
        </div>
        
        <div class="tempo-total">
          <div class="item-total">
            <span class="valor-total">{{ formatarNumero(estatisticasTotais.totalDias) }}</span>
            <span class="rotulo-total">dias totais</span>
          </div>
          <div class="item-total">
            <span class="valor-total">{{ formatarNumero(estatisticasTotais.totalHoras) }}</span>
            <span class="rotulo-total">horas totais</span>
          </div>
          <div class="item-total">
            <span class="valor-total">{{ formatarNumero(estatisticasTotais.totalMinutos) }}</span>
            <span class="rotulo-total">minutos totais</span>
          </div>
        </div>
      </div>

      <!-- Galeria de Fotos -->
      <GaleriaFotos />

      <!-- Seção de Perfis -->
      <SecaoPerfis />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { useTheme } from '../composables/useTheme'
import GaleriaFotos from '../components/PhotoGallery.vue'
import SecaoPerfis from '../components/ProfileSection.vue'

const roteador = useRouter()
const { isDark: temaEscuro } = useTheme()

interface DadosContagem {
  anos: number
  meses: number
  dias: number
  horas: number
  minutos: number
  segundos: number
  milissegundos: number
}

interface EstatisticasTotais {
  totalDias: number
  totalHoras: number
  totalMinutos: number
  totalSegundos: number
  totalMilissegundos: number
}

const contagem = ref<DadosContagem>({
  anos: 0,
  meses: 0,
  dias: 0,
  horas: 0,
  minutos: 0,
  segundos: 0,
  milissegundos: 0
})

const estatisticasTotais = ref<EstatisticasTotais>({
  totalDias: 0,
  totalHoras: 0,
  totalMinutos: 0,
  totalSegundos: 0,
  totalMilissegundos: 0
})

let idIntervalo: number | null = null

const dataInicial = new Date('2022-03-21T00:00:00.000Z')

const atualizarContagem = () => {
  const agora = new Date()
  const diferencaTempo = agora.getTime() - dataInicial.getTime()
  
  const anos = Math.floor(diferencaTempo / (365.25 * 24 * 60 * 60 * 1000))
  const meses = Math.floor((diferencaTempo % (365.25 * 24 * 60 * 60 * 1000)) / (30.44 * 24 * 60 * 60 * 1000))
  const dias = Math.floor((diferencaTempo % (30.44 * 24 * 60 * 60 * 1000)) / (24 * 60 * 60 * 1000))
  const horas = Math.floor((diferencaTempo % (24 * 60 * 60 * 1000)) / (60 * 60 * 1000))
  const minutos = Math.floor((diferencaTempo % (60 * 60 * 1000)) / (60 * 1000))
  const segundos = Math.floor((diferencaTempo % (60 * 1000)) / 1000)
  const milissegundos = diferencaTempo % 1000
  
  contagem.value = {
    anos,
    meses,
    dias,
    horas,
    minutos,
    segundos,
    milissegundos
  }
  
  estatisticasTotais.value = {
    totalDias: Math.floor(diferencaTempo / (24 * 60 * 60 * 1000)),
    totalHoras: Math.floor(diferencaTempo / (60 * 60 * 1000)),
    totalMinutos: Math.floor(diferencaTempo / (60 * 1000)),
    totalSegundos: Math.floor(diferencaTempo / 1000),
    totalMilissegundos: diferencaTempo
  }
}

const formatarNumero = (numero: number): string => {
  return numero.toLocaleString('pt-BR')
}

const voltarPagina = () => {
  roteador.push('/')
}

onMounted(() => {
  atualizarContagem()
  idIntervalo = setInterval(atualizarContagem, 1)
})

onUnmounted(() => {
  if (idIntervalo) {
    clearInterval(idIntervalo)
  }
})
</script>

<style scoped>
.container-contagem {
  min-height: 100vh;
  background: linear-gradient(135deg, #FFF0F5 0%, #E6E6FA 30%, #A9A9E1 70%, #FFC0CB 100%);
  position: relative;
  overflow: hidden;
  transition: all 0.3s ease;
}

.container-contagem.dark {
  background: linear-gradient(135deg, #111827 0%, #1F2937 30%, #374151 70%, #4B5563 100%);
}

.animacao-fundo {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  pointer-events: none;
  z-index: 1;
}

.elemento-flutuante {
  position: absolute;
  font-size: 1.5rem;
  opacity: 0.2;
  animation: flutuar 12s ease-in-out infinite;
}

.elemento-1 {
  top: 10%;
  left: 5%;
  animation-delay: 0s;
}

.elemento-2 {
  top: 20%;
  right: 10%;
  animation-delay: 2.4s;
}

.elemento-3 {
  bottom: 30%;
  left: 8%;
  animation-delay: 4.8s;
}

.elemento-4 {
  bottom: 15%;
  right: 5%;
  animation-delay: 7.2s;
}

.elemento-5 {
  top: 50%;
  left: 50%;
  animation-delay: 9.6s;
}

.conteudo {
  position: relative;
  z-index: 2;
  padding: 2rem;
  max-width: 1200px;
  margin: 0 auto;
}

.cabecalho {
  text-align: center;
  margin-bottom: 3rem;
}

.botao-voltar {
  position: absolute;
  top: 2rem;
  left: 2rem;
  background: var(--fundo-botao);
  border: none;
  padding: 0.75rem 1.5rem;
  border-radius: 25px;
  cursor: pointer;
  font-weight: 600;
  color: var(--cor-botao);
  transition: all 0.3s ease;
  backdrop-filter: blur(10px);
  box-shadow: var(--sombra-botao);
}

.botao-voltar:hover {
  transform: translateY(-2px);
  box-shadow: var(--sombra-botao-hover);
}

.titulo-pagina {
  font-size: 3rem;
  font-weight: 700;
  margin-bottom: 1rem;
}

.gradiente-titulo {
  background: linear-gradient(45deg, #EC4899, #8B5CF6);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.subtitulo-pagina {
  font-size: 1.2rem;
  color: var(--cor-subtitulo);
  margin-bottom: 2rem;
}

.secao-contagem {
  background: var(--fundo-secao);
  backdrop-filter: blur(15px);
  border-radius: 30px;
  padding: 3rem;
  margin-bottom: 3rem;
  box-shadow: var(--sombra-secao);
  border: 1px solid var(--borda-secao);
}

.grade-contagem {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(140px, 1fr));
  gap: 2rem;
  margin-bottom: 3rem;
}

.unidade-tempo {
  text-align: center;
  background: var(--fundo-unidade);
  border-radius: 20px;
  padding: 2rem 1rem;
  backdrop-filter: blur(10px);
  border: 1px solid var(--borda-unidade);
  transition: transform 0.3s ease;
}

.unidade-tempo:hover {
  transform: translateY(-5px);
}

.unidade-tempo.milissegundos {
  background: linear-gradient(135deg, #EC4899, #8B5CF6);
  color: white;
}

.valor-tempo {
  font-size: 2.5rem;
  font-weight: 700;
  line-height: 1;
  margin-bottom: 0.5rem;
  color: var(--cor-valor-tempo);
}

.unidade-tempo.milissegundos .valor-tempo {
  color: white;
}

.rotulo-tempo {
  font-size: 0.9rem;
  font-weight: 500;
  text-transform: uppercase;
  letter-spacing: 1px;
  opacity: 0.8;
  color: var(--cor-rotulo-tempo);
}

.unidade-tempo.milissegundos .rotulo-tempo {
  color: white;
}

.tempo-total {
  display: flex;
  justify-content: center;
  gap: 3rem;
  flex-wrap: wrap;
  padding-top: 2rem;
  border-top: 1px solid var(--cor-borda);
}

.item-total {
  text-align: center;
}

.valor-total {
  display: block;
  font-size: 1.5rem;
  font-weight: 700;
  color: #EC4899;
  margin-bottom: 0.25rem;
}

.rotulo-total {
  font-size: 0.85rem;
  color: var(--cor-rotulo-total);
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

@keyframes flutuar {
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
  --fundo-botao: rgba(255, 255, 255, 0.9);
  --cor-botao: #6B46C1;
  --sombra-botao: 0 4px 15px rgba(0, 0, 0, 0.1);
  --sombra-botao-hover: 0 6px 20px rgba(0, 0, 0, 0.15);
  --cor-subtitulo: #6B7280;
  --fundo-secao: rgba(255, 255, 255, 0.1);
  --sombra-secao: 0 20px 40px rgba(0, 0, 0, 0.1);
  --borda-secao: rgba(255, 255, 255, 0.2);
  --fundo-unidade: rgba(255, 255, 255, 0.2);
  --borda-unidade: rgba(255, 255, 255, 0.3);
  --cor-valor-tempo: #374151;
  --cor-rotulo-tempo: #6B7280;
  --cor-borda: rgba(255, 255, 255, 0.3);
  --cor-rotulo-total: #6B7280;
}

.dark {
  --fundo-botao: rgba(0, 0, 0, 0.3);
  --cor-botao: #E5E7EB;
  --sombra-botao: 0 4px 15px rgba(0, 0, 0, 0.3);
  --sombra-botao-hover: 0 6px 20px rgba(0, 0, 0, 0.4);
  --cor-subtitulo: #D1D5DB;
  --fundo-secao: rgba(0, 0, 0, 0.2);
  --sombra-secao: 0 20px 40px rgba(0, 0, 0, 0.3);
  --borda-secao: rgba(255, 255, 255, 0.1);
  --fundo-unidade: rgba(0, 0, 0, 0.2);
  --borda-unidade: rgba(255, 255, 255, 0.1);
  --cor-valor-tempo: #F9FAFB;
  --cor-rotulo-tempo: #D1D5DB;
  --cor-borda: rgba(255, 255, 255, 0.1);
  --cor-rotulo-total: #D1D5DB;
}

/* Design Responsivo */
@media (max-width: 768px) {
  .conteudo {
    padding: 1rem;
  }
  
  .botao-voltar {
    position: relative;
    top: 0;
    left: 0;
    margin-bottom: 2rem;
  }
  
  .titulo-pagina {
    font-size: 2.5rem;
  }
  
  .secao-contagem {
    padding: 2rem;
  }
  
  .grade-contagem {
    grid-template-columns: repeat(auto-fit, minmax(120px, 1fr));
    gap: 1.5rem;
  }
  
  .unidade-tempo {
    padding: 1.5rem 0.75rem;
  }
  
  .valor-tempo {
    font-size: 2rem;
  }
  
  .tempo-total {
    gap: 2rem;
  }
}

@media (max-width: 480px) {
  .titulo-pagina {
    font-size: 2rem;
  }
  
  .grade-contagem {
    grid-template-columns: repeat(2, 1fr);
  }
  
  .valor-tempo {
    font-size: 1.75rem;
  }
  
  .tempo-total {
    flex-direction: column;
    gap: 1rem;
  }
}
</style>