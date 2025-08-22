<template>
  <div class="galeria-fotos">
    <div class="cabecalho-galeria">
      <h2 class="titulo-galeria">
        <span class="icone-titulo">📸</span>
        Galeria de Momentos Especiais
      </h2>
      <p class="descricao-galeria">
        Adicione e preserve suas memórias mais preciosas
      </p>
      
      <!-- Indicador de sincronização de fotos -->
      <div v-if="estaCarregandoFotos" class="indicador-sincronizacao-fotos">
        <div class="spinner-sincronizacao"></div>
        <span class="texto-sincronizacao">Carregando fotos...</span>
      </div>
    </div>

    <div class="secao-upload">
      <div class="area-upload" @click="acionarInputArquivo" @dragover.prevent @drop.prevent="lidarComArrastar">
        <input 
          ref="inputArquivo" 
          type="file" 
          multiple 
          accept="image/*" 
          @change="lidarComSelecaoArquivo"
          class="input-arquivo-oculto"
        >
        <div class="conteudo-upload">
          <div class="container-icone-upload">
            <div class="icone-upload">📷</div>
            <div class="indicador-clique">👆</div>
          </div>
          <h3 class="titulo-upload">Adicionar Fotos</h3>
          <div class="instrucoes-upload">
            <p class="instrucao-principal">
              <strong>👆 CLIQUE AQUI</strong> para selecionar suas fotos
            </p>
            <p class="instrucao-secundaria">
              ou arraste e solte suas imagens nesta área
            </p>
          </div>
          <div class="detalhes-upload">
            <span class="item-detalhe">📁 Formatos: JPG, PNG, GIF</span>
            <span class="item-detalhe">📏 Máximo: 5MB cada</span>
            <span class="item-detalhe">🖼️ Múltiplas fotos aceitas</span>
          </div>
          <div class="botao-visual-upload">
            <span class="texto-botao">Clique para Escolher Fotos</span>
          </div>
        </div>
      </div>
    </div>

    <!-- Indicador de carregamento -->
    <div v-if="estaEnviando" class="indicador-carregamento">
      <div class="spinner-carregamento"></div>
      <p>Salvando suas fotos...</p>
    </div>

    <div v-if="fotos.length > 0" class="grade-fotos">
      <div 
        v-for="(foto, indice) in fotos" 
        :key="foto.id"
        class="item-foto"
        @click="abrirLightbox(indice)"
      >
        <img :src="foto.url" :alt="foto.nome" class="imagem-foto">
        <div class="sobreposicao-foto">
          <div class="acoes-foto">
            <button @click.stop="baixarFoto(foto)" class="botao-acao botao-baixar" title="Baixar">
              📥
            </button>
            <button @click.stop="deletarFoto(foto.id)" class="botao-acao botao-deletar" title="Excluir">
              🗑️
            </button>
          </div>
          <div class="informacoes-foto">
            <span class="nome-foto">{{ foto.nome }}</span>
            <span class="data-foto">{{ formatarData(foto.dataEnvio) }}</span>
          </div>
        </div>
      </div>
    </div>

    <div v-else-if="!estaEnviando && !estaCarregandoFotos" class="galeria-vazia">
      <div class="icone-vazio">🖼️</div>
      <h3>Nenhuma foto ainda</h3>
      <p>Adicione suas primeiras memórias especiais!</p>
      <div class="chamada-vazia">
        <span class="seta-chamada">👆</span>
        <span class="texto-chamada">Clique na área acima para começar</span>
      </div>
    </div>

    <!-- Modal Lightbox -->
    <div v-if="lightboxAberto" class="sobreposicao-lightbox" @click="fecharLightbox">
      <div class="conteudo-lightbox" @click.stop>
        <button class="fechar-lightbox" @click="fecharLightbox">×</button>
        <button class="navegacao-lightbox anterior" @click="fotoAnterior" v-if="fotos.length > 1">‹</button>
        <button class="navegacao-lightbox proxima" @click="proximaFoto" v-if="fotos.length > 1">›</button>
        
        <img 
          :src="fotos[indiceFotoAtual]?.url" 
          :alt="fotos[indiceFotoAtual]?.nome"
          class="imagem-lightbox"
        >
        
        <div class="informacoes-lightbox">
          <h3>{{ fotos[indiceFotoAtual]?.nome }}</h3>
          <p>{{ formatarData(fotos[indiceFotoAtual]?.dataEnvio) }}</p>
          <span class="contador-foto">{{ indiceFotoAtual + 1 }} de {{ fotos.length }}</span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import { apiFotos, type Foto } from '../services/api'

// Estados reativos
const fotos = ref<Foto[]>([])
const inputArquivo = ref<HTMLInputElement>()
const lightboxAberto = ref(false)
const indiceFotoAtual = ref(0)
const estaEnviando = ref(false)
const estaCarregandoFotos = ref(false)

// Intervalo para sincronização automática
let intervaloSincronizacao: number | null = null
const INTERVALO_SINCRONIZACAO_FOTOS = 10000 // 10 segundos

// Carrega fotos do servidor quando o componente é montado
onMounted(async () => {
  await carregarFotosDoServidor()
  iniciarSincronizacaoFotos()
})

// Para a sincronização quando o componente é desmontado
onUnmounted(() => {
  pararSincronizacaoFotos()
})

// Função para iniciar sincronização automática de fotos
const iniciarSincronizacaoFotos = () => {
  if (intervaloSincronizacao) {
    clearInterval(intervaloSincronizacao)
  }

  intervaloSincronizacao = setInterval(() => {
    carregarFotosDoServidor(true) // Carregamento silencioso
  }, INTERVALO_SINCRONIZACAO_FOTOS)

  console.log('🖼️ Sincronização automática de fotos iniciada')
}

// Função para parar sincronização automática de fotos
const pararSincronizacaoFotos = () => {
  if (intervaloSincronizacao) {
    clearInterval(intervaloSincronizacao)
    intervaloSincronizacao = null
    console.log('⏹️ Sincronização de fotos parada')
  }
}

// Função para carregar fotos do servidor
const carregarFotosDoServidor = async (silencioso = false) => {
  try {
    if (!silencioso) {
      estaCarregandoFotos.value = true
    }

    const fotosCarregadas = await apiFotos.obterTodasFotos()
    
    // Verificar se há novas fotos
    const fotosNovas = fotosCarregadas.filter(fotoNova => 
      !fotos.value.some(fotoExistente => fotoExistente.id === fotoNova.id)
    )

    if (fotosNovas.length > 0 && fotos.value.length > 0) {
      console.log(`📸 ${fotosNovas.length} nova(s) foto(s) encontrada(s)`)
    }

    fotos.value = fotosCarregadas
    
    if (!silencioso) {
      console.log('✅ Fotos carregadas:', fotos.value.length)
    }
  } catch (erro) {
    console.error('❌ Erro ao carregar fotos:', erro)
    if (!silencioso) {
      alert('Erro ao carregar fotos. Verifique se o servidor está funcionando.')
    }
  } finally {
    if (!silencioso) {
      estaCarregandoFotos.value = false
    }
  }
}

// Função para acionar input de arquivo
const acionarInputArquivo = () => {
  inputArquivo.value?.click()
}

// Função para lidar com seleção de arquivo
const lidarComSelecaoArquivo = (evento: Event) => {
  const alvo = evento.target as HTMLInputElement
  if (alvo.files) {
    lidarComArquivos(Array.from(alvo.files))
  }
}

// Função para lidar com arrastar e soltar
const lidarComArrastar = (evento: DragEvent) => {
  if (evento.dataTransfer?.files) {
    lidarComArquivos(Array.from(evento.dataTransfer.files))
  }
}

// Função principal para lidar com arquivos
const lidarComArquivos = async (arquivos: File[]) => {
  const arquivosValidos = arquivos.filter(arquivo => {
    if (!arquivo.type.startsWith('image/')) {
      alert(`Arquivo ${arquivo.name} não é uma imagem válida.`)
      return false
    }
    if (arquivo.size > 5 * 1024 * 1024) {
      alert(`Arquivo ${arquivo.name} é muito grande (máximo 5MB).`)
      return false
    }
    return true
  })

  if (arquivosValidos.length === 0) return

  estaEnviando.value = true

  try {
    const fotosEnviadas = await apiFotos.enviarFotos(arquivosValidos)
    
    // Adicionar novas fotos ao início da lista
    fotos.value.unshift(...fotosEnviadas)
    
    // Limpar input de arquivo
    if (inputArquivo.value) {
      inputArquivo.value.value = ''
    }

    console.log('✅ Fotos enviadas com sucesso:', fotosEnviadas.length)
    
    // Recarregar fotos após um breve delay para garantir sincronização
    setTimeout(() => {
      carregarFotosDoServidor(true)
    }, 2000)
    
  } catch (erro) {
    console.error('❌ Erro ao fazer upload:', erro)
    alert('Erro ao salvar as fotos. Tente novamente.')
  } finally {
    estaEnviando.value = false
  }
}

// Função para deletar foto
const deletarFoto = async (idFoto: string) => {
  if (confirm('Tem certeza que deseja excluir esta foto permanentemente?')) {
    try {
      await apiFotos.deletarFoto(idFoto)
      fotos.value = fotos.value.filter(foto => foto.id !== idFoto)
      console.log('✅ Foto deletada com sucesso')
      
      // Recarregar fotos para sincronizar com outros usuários
      setTimeout(() => {
        carregarFotosDoServidor(true)
      }, 1000)
      
    } catch (erro) {
      console.error('❌ Erro ao deletar foto:', erro)
      alert('Erro ao excluir a foto. Tente novamente.')
    }
  }
}

// Função para baixar foto
const baixarFoto = (foto: Foto) => {
  const link = document.createElement('a')
  link.href = foto.url
  link.download = foto.nome
  link.click()
}

// Função para abrir lightbox
const abrirLightbox = (indice: number) => {
  indiceFotoAtual.value = indice
  lightboxAberto.value = true
  document.body.style.overflow = 'hidden'
}

// Função para fechar lightbox
const fecharLightbox = () => {
  lightboxAberto.value = false
  document.body.style.overflow = 'auto'
}

// Função para próxima foto
const proximaFoto = () => {
  indiceFotoAtual.value = (indiceFotoAtual.value + 1) % fotos.value.length
}

// Função para foto anterior
const fotoAnterior = () => {
  indiceFotoAtual.value = indiceFotoAtual.value === 0 
    ? fotos.value.length - 1 
    : indiceFotoAtual.value - 1
}

// Função para formatar data
const formatarData = (data: Date | string): string => {
  const d = typeof data === 'string' ? new Date(data) : data
  return d.toLocaleDateString('pt-BR', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}
</script>

<style scoped>
/* Container principal */
.galeria-fotos {
  width: 100%;
  max-width: 1200px;
  margin: 0 auto;
  padding: 2rem;
}

/* Cabeçalho da galeria */
.cabecalho-galeria {
  text-align: center;
  margin-bottom: 3rem;
}

.titulo-galeria {
  font-size: 2.5rem;
  font-weight: 700;
  color: var(--texto-primario);
  margin-bottom: 1rem;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 1rem;
}

.icone-titulo {
  font-size: 2rem;
}

.descricao-galeria {
  font-size: 1.1rem;
  color: var(--texto-secundario);
  line-height: 1.6;
  margin-bottom: 1rem;
}

/* Indicador de sincronização de fotos */
.indicador-sincronizacao-fotos {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  margin-bottom: 1rem;
  padding: 0.5rem 1rem;
  background: var(--fundo-sincronizacao);
  border-radius: 15px;
  border: 1px solid var(--borda-sincronizacao);
}

.spinner-sincronizacao {
  width: 14px;
  height: 14px;
  border: 2px solid var(--cor-spinner-track);
  border-top: 2px solid var(--cor-primaria);
  border-radius: 50%;
  animation: girar 1s linear infinite;
}

.texto-sincronizacao {
  font-size: 0.85rem;
  color: var(--texto-sincronizacao);
  font-weight: 500;
}

/* Seção de upload */
.secao-upload {
  margin-bottom: 3rem;
}

.area-upload {
  border: 3px dashed var(--cor-primaria);
  border-radius: 20px;
  padding: 3rem;
  text-align: center;
  cursor: pointer;
  transition: all 0.3s ease;
  background: var(--fundo-upload);
  position: relative;
  overflow: hidden;
}

.area-upload::before {
  content: '';
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(90deg, transparent, rgba(236, 72, 153, 0.1), transparent);
  animation: brilho 2s infinite;
}

.area-upload:hover {
  border-color: var(--cor-primaria);
  background: var(--fundo-upload-hover);
  transform: translateY(-2px);
  box-shadow: 0 10px 30px var(--cor-sombra);
}

.conteudo-upload {
  position: relative;
  z-index: 2;
}

.input-arquivo-oculto {
  display: none;
}

.container-icone-upload {
  position: relative;
  display: inline-block;
  margin-bottom: 1.5rem;
}

.icone-upload {
  font-size: 4rem;
  margin-bottom: 0.5rem;
  animation: pular 2s infinite;
}

.indicador-clique {
  position: absolute;
  top: -10px;
  right: -10px;
  font-size: 1.5rem;
  animation: pulsar-ponto 1.5s infinite;
}

.titulo-upload {
  font-size: 1.8rem;
  color: var(--texto-primario);
  margin-bottom: 1rem;
  font-weight: 700;
}

.instrucoes-upload {
  margin-bottom: 1.5rem;
}

.instrucao-principal {
  font-size: 1.2rem;
  color: var(--cor-primaria);
  margin-bottom: 0.5rem;
  font-weight: 700;
  animation: brilhar 2s ease-in-out infinite alternate;
}

.instrucao-secundaria {
  color: var(--texto-secundario);
  font-size: 1rem;
}

.detalhes-upload {
  display: flex;
  justify-content: center;
  gap: 1rem;
  flex-wrap: wrap;
  margin-bottom: 1.5rem;
}

.item-detalhe {
  background: var(--fundo-detalhe);
  padding: 0.5rem 1rem;
  border-radius: 20px;
  font-size: 0.9rem;
  color: var(--texto-secundario);
  border: 1px solid var(--borda-detalhe);
}

.botao-visual-upload {
  background: linear-gradient(135deg, var(--cor-primaria), #8B5CF6);
  color: white;
  padding: 1rem 2rem;
  border-radius: 25px;
  display: inline-block;
  font-weight: 600;
  font-size: 1.1rem;
  box-shadow: 0 5px 15px rgba(236, 72, 153, 0.3);
  transition: all 0.3s ease;
  animation: pulsar-botao 2s infinite;
}

.area-upload:hover .botao-visual-upload {
  transform: scale(1.05);
  box-shadow: 0 8px 25px rgba(236, 72, 153, 0.4);
}

.texto-botao {
  display: block;
}

/* Indicador de carregamento */
.indicador-carregamento {
  text-align: center;
  padding: 2rem;
  color: var(--texto-primario);
}

.spinner-carregamento {
  width: 40px;
  height: 40px;
  border: 4px solid var(--cor-borda);
  border-top: 4px solid var(--cor-primaria);
  border-radius: 50%;
  animation: girar 1s linear infinite;
  margin: 0 auto 1rem;
}

/* Grade de fotos */
.grade-fotos {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
  gap: 2rem;
}

.item-foto {
  position: relative;
  aspect-ratio: 1;
  border-radius: 15px;
  overflow: hidden;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 8px 25px var(--cor-sombra);
}

.item-foto:hover {
  transform: translateY(-5px);
  box-shadow: 0 15px 35px var(--cor-sombra-hover);
}

.imagem-foto {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.3s ease;
}

.item-foto:hover .imagem-foto {
  transform: scale(1.05);
}

.sobreposicao-foto {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: linear-gradient(
    to bottom,
    rgba(0, 0, 0, 0.1) 0%,
    rgba(0, 0, 0, 0.1) 60%,
    rgba(0, 0, 0, 0.8) 100%
  );
  opacity: 0;
  transition: opacity 0.3s ease;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  padding: 1rem;
}

.item-foto:hover .sobreposicao-foto {
  opacity: 1;
}

.acoes-foto {
  display: flex;
  gap: 0.5rem;
  justify-content: flex-end;
}

.botao-acao {
  background: rgba(255, 255, 255, 0.9);
  border: none;
  border-radius: 50%;
  width: 40px;
  height: 40px;
  cursor: pointer;
  font-size: 1.2rem;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  justify-content: center;
}

.botao-acao:hover {
  background: white;
  transform: scale(1.1);
}

.botao-deletar:hover {
  background: #ff4757;
  color: white;
}

.botao-baixar:hover {
  background: #2ed573;
  color: white;
}

.informacoes-foto {
  color: white;
  text-align: left;
}

.nome-foto {
  display: block;
  font-weight: 600;
  margin-bottom: 0.25rem;
  font-size: 0.9rem;
}

.data-foto {
  font-size: 0.8rem;
  opacity: 0.8;
}

/* Estado vazio */
.galeria-vazia {
  text-align: center;
  padding: 4rem 2rem;
  color: var(--texto-secundario);
}

.icone-vazio {
  font-size: 4rem;
  margin-bottom: 1rem;
  opacity: 0.5;
}

.galeria-vazia h3 {
  font-size: 1.5rem;
  margin-bottom: 0.5rem;
  color: var(--texto-primario);
}

.chamada-vazia {
  margin-top: 2rem;
  padding: 1rem;
  background: var(--fundo-chamada);
  border-radius: 15px;
  border: 2px dashed var(--cor-primaria);
}

.seta-chamada {
  font-size: 1.5rem;
  display: block;
  margin-bottom: 0.5rem;
  animation: pular-seta 1.5s infinite;
}

.texto-chamada {
  font-weight: 600;
  color: var(--cor-primaria);
}

/* Lightbox */
.sobreposicao-lightbox {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.9);
  z-index: 2000;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 2rem;
}

.conteudo-lightbox {
  position: relative;
  max-width: 90vw;
  max-height: 90vh;
  display: flex;
  flex-direction: column;
  align-items: center;
}

.imagem-lightbox {
  max-width: 100%;
  max-height: 70vh;
  object-fit: contain;
  border-radius: 10px;
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.5);
}

.fechar-lightbox {
  position: absolute;
  top: -50px;
  right: -10px;
  background: rgba(255, 255, 255, 0.2);
  border: none;
  color: white;
  font-size: 2rem;
  width: 40px;
  height: 40px;
  border-radius: 50%;
  cursor: pointer;
  transition: all 0.3s ease;
}

.fechar-lightbox:hover {
  background: rgba(255, 255, 255, 0.3);
}

.navegacao-lightbox {
  position: absolute;
  top: 50%;
  transform: translateY(-50%);
  background: rgba(255, 255, 255, 0.2);
  border: none;
  color: white;
  font-size: 2rem;
  width: 50px;
  height: 50px;
  border-radius: 50%;
  cursor: pointer;
  transition: all 0.3s ease;
}

.navegacao-lightbox:hover {
  background: rgba(255, 255, 255, 0.3);
}

.navegacao-lightbox.anterior {
  left: -70px;
}

.navegacao-lightbox.proxima {
  right: -70px;
}

.informacoes-lightbox {
  text-align: center;
  color: white;
  margin-top: 1rem;
}

.informacoes-lightbox h3 {
  margin-bottom: 0.5rem;
}

.contador-foto {
  display: block;
  margin-top: 0.5rem;
  opacity: 0.7;
  font-size: 0.9rem;
}

/* Animações */
@keyframes brilho {
  0% { left: -100%; }
  100% { left: 100%; }
}

@keyframes pular {
  0%, 20%, 50%, 80%, 100% {
    transform: translateY(0);
  }
  40% {
    transform: translateY(-10px);
  }
  60% {
    transform: translateY(-5px);
  }
}

@keyframes pulsar-ponto {
  0%, 100% {
    transform: scale(1);
    opacity: 1;
  }
  50% {
    transform: scale(1.2);
    opacity: 0.7;
  }
}

@keyframes brilhar {
  from {
    text-shadow: 0 0 5px var(--cor-primaria);
  }
  to {
    text-shadow: 0 0 20px var(--cor-primaria), 0 0 30px var(--cor-primaria);
  }
}

@keyframes pulsar-botao {
  0%, 100% {
    transform: scale(1);
  }
  50% {
    transform: scale(1.02);
  }
}

@keyframes pular-seta {
  0%, 20%, 50%, 80%, 100% {
    transform: translateY(0);
  }
  40% {
    transform: translateY(-8px);
  }
  60% {
    transform: translateY(-4px);
  }
}

@keyframes girar {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

/* Design responsivo */
@media (max-width: 768px) {
  .galeria-fotos {
    padding: 1rem;
  }
  
  .titulo-galeria {
    font-size: 2rem;
  }
  
  .grade-fotos {
    grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
    gap: 1rem;
  }
  
  .area-upload {
    padding: 2rem 1rem;
  }
  
  .detalhes-upload {
    flex-direction: column;
    align-items: center;
  }
  
  .navegacao-lightbox {
    display: none;
  }
  
  .fechar-lightbox {
    top: -40px;
    right: 0;
  }
}

/* Variáveis CSS para temas */
:root {
  --texto-primario: #374151;
  --texto-secundario: #6B7280;
  --texto-terciario: #9CA3AF;
  --cor-primaria: #EC4899;
  --cor-borda: #E5E7EB;
  --fundo-upload: rgba(255, 255, 255, 0.1);
  --fundo-upload-hover: rgba(255, 255, 255, 0.2);
  --cor-sombra: rgba(0, 0, 0, 0.1);
  --cor-sombra-hover: rgba(0, 0, 0, 0.15);
  --fundo-detalhe: rgba(255, 255, 255, 0.5);
  --borda-detalhe: rgba(255, 255, 255, 0.3);
  --fundo-chamada: rgba(255, 255, 255, 0.1);
  --fundo-sincronizacao: rgba(255, 255, 255, 0.7);
  --borda-sincronizacao: rgba(0, 0, 0, 0.1);
  --texto-sincronizacao: #6B7280;
  --cor-spinner-track: rgba(0, 0, 0, 0.1);
}

:global(.dark-theme) {
  --texto-primario: #F9FAFB;
  --texto-secundario: #D1D5DB;
  --texto-terciario: #9CA3AF;
  --cor-primaria: #EC4899;
  --cor-borda: #374151;
  --fundo-upload: rgba(0, 0, 0, 0.2);
  --fundo-upload-hover: rgba(0, 0, 0, 0.3);
  --cor-sombra: rgba(0, 0, 0, 0.3);
  --cor-sombra-hover: rgba(0, 0, 0, 0.4);
  --fundo-detalhe: rgba(0, 0, 0, 0.3);
  --borda-detalhe: rgba(255, 255, 255, 0.1);
  --fundo-chamada: rgba(0, 0, 0, 0.2);
  --fundo-sincronizacao: rgba(0, 0, 0, 0.3);
  --borda-sincronizacao: rgba(255, 255, 255, 0.1);
  --texto-sincronizacao: #D1D5DB;
  --cor-spinner-track: rgba(255, 255, 255, 0.1);
}
</style>