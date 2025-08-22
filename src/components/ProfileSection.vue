<template>
  <div class="secao-perfis">
    <div class="cabecalho-perfis">
      <h2 class="titulo-perfis">
        <span class="icone-titulo">💕</span>
        Perfis Especiais
      </h2>
      <p class="descricao-perfis">
        Conheça as pessoas especiais e deixe mensagens de carinho
      </p>
      
      <!-- Indicador de sincronização -->
      <div v-if="estaCarregandoMensagens" class="indicador-sincronizacao">
        <div class="spinner-sincronizacao"></div>
        <span class="texto-sincronizacao">Sincronizando mensagens...</span>
      </div>
      
      <!-- Status da última atualização -->
      <div class="status-atualizacao">
        <span class="icone-status">🔄</span>
        <span class="texto-status">
          Última atualização: {{ formatarUltimaAtualizacao() }}
        </span>
      </div>
    </div>

    <div class="grade-perfis">
      <div 
        v-for="perfil in perfis" 
        :key="perfil.id"
        class="cartao-perfil"
        :style="{ '--cor-perfil': perfil.cor }"
      >
        <!-- Cabeçalho do Perfil -->
        <div class="cabecalho-cartao">
          <div class="container-avatar">
            <div class="avatar-perfil">
              <img 
                :src="perfil.fotoPerfil" 
                :alt="`Foto de ${perfil.nome}`"
                class="imagem-avatar"
                @error="tratarErroImagem"
              >
              <div class="sobreposicao-avatar">
                <input 
                  type="file" 
                  :ref="`inputFoto${perfil.id}`"
                  accept="image/*"
                  @change="(evento) => alterarFotoPerfil(perfil.id, evento)"
                  class="input-foto-oculto"
                >
                <button 
                  @click="acionarInputFoto(perfil.id)"
                  class="botao-alterar-foto"
                  title="Alterar foto"
                >
                  📷
                </button>
              </div>
            </div>
            <div class="emoji-perfil">{{ perfil.emoji }}</div>
          </div>
          
          <div class="informacoes-basicas">
            <h3 class="nome-perfil">{{ perfil.nome }}</h3>
            <div class="detalhes-perfil">
              <p class="idade-perfil">{{ perfil.idade }} anos</p>
              <p class="aniversario-perfil">
                🎂 {{ formatarAniversario(perfil.aniversario) }}
              </p>
              <p class="profissao-perfil">
                💼 {{ perfil.profissao }}
              </p>
              <p class="cidade-perfil">
                📍 {{ perfil.cidadeNatal }}
              </p>
            </div>
          </div>
        </div>

        <!-- Descrição do Perfil -->
        <div class="descricao-perfil">
          <p>{{ perfil.descricao }}</p>
        </div>

        <!-- Hobbies -->
        <div class="secao-hobbies">
          <h4 class="titulo-hobbies">
            <span class="icone-hobbies">🎯</span>
            Interesses
          </h4>
          <div class="lista-hobbies">
            <span 
              v-for="hobby in perfil.hobbies" 
              :key="hobby"
              class="tag-hobby"
            >
              {{ hobby }}
            </span>
          </div>
        </div>

        <!-- Seção de Mensagem -->
        <div class="secao-mensagem">
          <h4 class="titulo-mensagem">
            <span class="icone-mensagem">💌</span>
            Enviar Mensagem de Carinho
          </h4>
          
          <div class="container-input-mensagem">
            <textarea
              v-model="mensagensRascunho[perfil.id]"
              :placeholder="`Escreva uma mensagem carinhosa para ${perfil.nome}...`"
              class="campo-mensagem"
              rows="3"
              maxlength="500"
            ></textarea>
            <div class="contador-caracteres">
              {{ (mensagensRascunho[perfil.id] || '').length }}/500
            </div>
          </div>
          
          <button 
            @click="salvarMensagem(perfil.id)"
            :disabled="!mensagensRascunho[perfil.id]?.trim() || estaEnviandoMensagem[perfil.id]"
            class="botao-enviar-mensagem"
          >
            <span v-if="estaEnviandoMensagem[perfil.id]" class="spinner-pequeno"></span>
            <span v-else class="icone-botao">💌</span>
            {{ estaEnviandoMensagem[perfil.id] ? 'Enviando...' : 'Enviar Mensagem' }}
          </button>
        </div>

        <!-- Histórico de Mensagens -->
        <div class="historico-mensagens" v-if="obterMensagensPerfil(perfil.id).length > 0">
          <h4 class="titulo-historico">
            <span class="icone-historico">📝</span>
            Mensagens Enviadas ({{ contarMensagens(perfil.id) }})
            <span v-if="obterUltimaAtualizacao(perfil.id)" class="timestamp-atualizacao">
              • Atualizado {{ formatarTempoRelativo(obterUltimaAtualizacao(perfil.id)) }}
            </span>
          </h4>
          
          <div class="lista-mensagens">
            <div 
              v-for="mensagem in mensagensOrdenadas(perfil.id)" 
              :key="mensagem.id"
              class="item-mensagem"
            >
              <div class="cabecalho-mensagem">
                <span class="data-mensagem">{{ formatarDataMensagem(mensagem.data) }}</span>
                <span class="hora-mensagem">{{ formatarHoraMensagem(mensagem.dataCompleta) }}</span>
              </div>
              <div class="conteudo-mensagem">
                {{ mensagem.mensagem }}
              </div>
            </div>
          </div>
        </div>

        <!-- Estado Vazio -->
        <div v-else class="sem-mensagens">
          <div class="icone-sem-mensagens">💭</div>
          <p>Ainda não há mensagens para {{ perfil.nome }}</p>
          <p class="texto-incentivo">Seja o primeiro a enviar uma mensagem carinhosa!</p>
        </div>
      </div>
    </div>

    <!-- Indicador de carregamento para foto -->
    <div v-if="estaEnviandoFoto" class="indicador-carregamento-foto">
      <div class="spinner-carregamento"></div>
      <p>Salvando nova foto de perfil...</p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, reactive } from 'vue'
import { apiPerfis, type Perfil, type MensagemCarinho } from '../services/api'
import { messageSyncGlobal } from '../composables/useMessageSync'

// Estados reativos
const perfis = ref<Perfil[]>([])
const mensagensRascunho = reactive<Record<string, string>>({})
const estaEnviandoMensagem = reactive<Record<string, boolean>>({})
const estaEnviandoFoto = ref(false)

// Usar o sistema de sincronização global
const {
  ultimaAtualizacao,
  estaCarregando: estaCarregandoMensagens,
  adicionarMensagemLocal,
  iniciarSincronizacao,
  ativarAtualizacaoRapida,
  pararSincronizacao,
  obterMensagensPerfil,
  contarMensagens,
  obterUltimaAtualizacao
} = messageSyncGlobal

// Carrega perfis e inicia sincronização quando o componente é montado
onMounted(async () => {
  console.log('🚀 Montando componente ProfileSection...')
  await carregarPerfis()
  
  if (perfis.value.length > 0) {
    const idsPerfis = perfis.value.map(p => p.id)
    console.log('📋 IDs dos perfis encontrados:', idsPerfis)
    iniciarSincronizacao(idsPerfis)
  } else {
    console.warn('⚠️ Nenhum perfil encontrado para sincronização')
  }
})

// Para a sincronização quando o componente é desmontado
onUnmounted(() => {
  console.log('🛑 Desmontando componente ProfileSection...')
  pararSincronizacao()
})

// Função para carregar perfis do servidor
const carregarPerfis = async () => {
  try {
    console.log('🔄 Carregando perfis do servidor...')
    perfis.value = await apiPerfis.obterPerfis()
    
    // Inicializar rascunhos e estados de envio
    perfis.value.forEach(perfil => {
      mensagensRascunho[perfil.id] = ''
      estaEnviandoMensagem[perfil.id] = false
    })
    
    console.log('✅ Perfis carregados:', perfis.value.length)
    console.log('📋 Perfis:', perfis.value.map(p => ({ id: p.id, nome: p.nome })))
  } catch (erro) {
    console.error('❌ Erro ao carregar perfis:', erro)
    alert('Erro ao carregar perfis. Verifique se o servidor está funcionando.')
  }
}

// Função para salvar mensagem de carinho
const salvarMensagem = async (idPerfil: string) => {
  const mensagem = mensagensRascunho[idPerfil]?.trim()
  if (!mensagem) {
    console.warn('⚠️ Tentativa de enviar mensagem vazia')
    return
  }

  console.log(`📨 Enviando mensagem para ${idPerfil}:`, mensagem)
  estaEnviandoMensagem[idPerfil] = true

  try {
    const novaMensagem = await apiPerfis.salvarMensagemCarinho(idPerfil, mensagem)
    console.log('✅ Mensagem salva no servidor:', novaMensagem)
    
    // Adicionar mensagem ao estado global imediatamente
    adicionarMensagemLocal(idPerfil, novaMensagem)
    
    // Limpar rascunho
    mensagensRascunho[idPerfil] = ''
    
    // Ativar atualização rápida para sincronizar com outros usuários
    const idsPerfis = perfis.value.map(p => p.id)
    ativarAtualizacaoRapida(idsPerfis)
    
    console.log('✅ Mensagem enviada e sincronização rápida ativada')
    
    // Mostrar feedback de sucesso
    alert('Mensagem enviada com sucesso! 💕')
  } catch (erro) {
    console.error('❌ Erro ao salvar mensagem:', erro)
    alert('Erro ao enviar mensagem. Tente novamente.')
  } finally {
    estaEnviandoMensagem[idPerfil] = false
  }
}

// Função para acionar input de foto
const acionarInputFoto = (idPerfil: string) => {
  const input = document.querySelector(`input[ref="inputFoto${idPerfil}"]`) as HTMLInputElement
  if (input) {
    input.click()
  }
}

// Função para alterar foto de perfil
const alterarFotoPerfil = async (idPerfil: string, evento: Event) => {
  const alvo = evento.target as HTMLInputElement
  const arquivo = alvo.files?.[0]
  
  if (!arquivo) return

  // Validar tipo de arquivo
  if (!arquivo.type.startsWith('image/')) {
    alert('Por favor, selecione apenas arquivos de imagem.')
    return
  }

  // Validar tamanho do arquivo (máximo 5MB)
  if (arquivo.size > 5 * 1024 * 1024) {
    alert('A imagem deve ter no máximo 5MB.')
    return
  }

  estaEnviandoFoto.value = true

  try {
    const resultado = await apiPerfis.enviarFotoPerfil(idPerfil, arquivo)
    
    // Atualizar foto do perfil na interface
    const perfilIndex = perfis.value.findIndex(p => p.id === idPerfil)
    if (perfilIndex !== -1) {
      perfis.value[perfilIndex].fotoPerfil = resultado.urlFoto
    }
    
    console.log('✅ Foto de perfil atualizada')
    alert('Foto de perfil atualizada com sucesso! 📸')
  } catch (erro) {
    console.error('❌ Erro ao alterar foto de perfil:', erro)
    alert('Erro ao alterar foto de perfil. Tente novamente.')
  } finally {
    estaEnviandoFoto.value = false
    // Limpar input
    alvo.value = ''
  }
}

// Função para tratar erro de imagem
const tratarErroImagem = (evento: Event) => {
  const img = evento.target as HTMLImageElement
  img.src = 'https://images.pexels.com/photos/771742/pexels-photo-771742.jpeg?auto=compress&cs=tinysrgb&w=400'
}

// Função para ordenar mensagens por data
const mensagensOrdenadas = (idPerfil: string): MensagemCarinho[] => {
  const mensagens = obterMensagensPerfil(idPerfil)
  console.log(`📋 Ordenando mensagens para ${idPerfil}:`, mensagens.length, 'mensagens')
  return [...mensagens].sort((a, b) => 
    new Date(b.dataCompleta).getTime() - new Date(a.dataCompleta).getTime()
  )
}

// Função para formatar aniversário
const formatarAniversario = (aniversario: string): string => {
  const [dia, mes, ano] = aniversario.split('/')
  const data = new Date(parseInt(ano), parseInt(mes) - 1, parseInt(dia))
  return data.toLocaleDateString('pt-BR', {
    day: '2-digit',
    month: 'long'
  })
}

// Função para formatar data da mensagem
const formatarDataMensagem = (data: string): string => {
  const dataObj = new Date(data)
  const hoje = new Date()
  const ontem = new Date(hoje)
  ontem.setDate(hoje.getDate() - 1)

  if (dataObj.toDateString() === hoje.toDateString()) {
    return 'Hoje'
  } else if (dataObj.toDateString() === ontem.toDateString()) {
    return 'Ontem'
  } else {
    return dataObj.toLocaleDateString('pt-BR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric'
    })
  }
}

// Função para formatar hora da mensagem
const formatarHoraMensagem = (dataCompleta: string): string => {
  const data = new Date(dataCompleta)
  return data.toLocaleTimeString('pt-BR', {
    hour: '2-digit',
    minute: '2-digit'
  })
}

// Função para formatar tempo relativo
const formatarTempoRelativo = (timestamp: string | null): string => {
  if (!timestamp) return 'nunca'
  
  const agora = new Date()
  const data = new Date(timestamp)
  const diferencaMs = agora.getTime() - data.getTime()
  const diferencaSegundos = Math.floor(diferencaMs / 1000)
  
  if (diferencaSegundos < 60) {
    return 'agora mesmo'
  } else if (diferencaSegundos < 3600) {
    const minutos = Math.floor(diferencaSegundos / 60)
    return `há ${minutos} minuto${minutos > 1 ? 's' : ''}`
  } else if (diferencaSegundos < 86400) {
    const horas = Math.floor(diferencaSegundos / 3600)
    return `há ${horas} hora${horas > 1 ? 's' : ''}`
  } else {
    const dias = Math.floor(diferencaSegundos / 86400)
    return `há ${dias} dia${dias > 1 ? 's' : ''}`
  }
}

// Função para formatar última atualização geral
const formatarUltimaAtualizacao = (): string => {
  const timestamps = Object.values(ultimaAtualizacao.value) as string[]
  if (timestamps.length === 0) return 'nunca'
  
  const maisRecente = timestamps.reduce((mais, atual) => {
    return new Date(atual) > new Date(mais) ? atual : mais
  })
  
  return formatarTempoRelativo(maisRecente)
}
</script>

<style scoped>
/* Container principal */
.secao-perfis {
  width: 100%;
  max-width: 1200px;
  margin: 0 auto;
  padding: 2rem;
}

/* Cabeçalho da seção */
.cabecalho-perfis {
  text-align: center;
  margin-bottom: 3rem;
}

.titulo-perfis {
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

.descricao-perfis {
  font-size: 1.1rem;
  color: var(--texto-secundario);
  line-height: 1.6;
  margin-bottom: 1rem;
}

/* Indicador de sincronização */
.indicador-sincronizacao {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  margin-bottom: 1rem;
  padding: 0.75rem 1.5rem;
  background: var(--fundo-sincronizacao);
  border-radius: 20px;
  border: 1px solid var(--borda-sincronizacao);
}

.spinner-sincronizacao {
  width: 16px;
  height: 16px;
  border: 2px solid var(--cor-spinner-track);
  border-top: 2px solid var(--cor-primaria);
  border-radius: 50%;
  animation: girar 1s linear infinite;
}

.texto-sincronizacao {
  font-size: 0.9rem;
  color: var(--texto-sincronizacao);
  font-weight: 500;
}

/* Status da atualização */
.status-atualizacao {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  margin-bottom: 1rem;
  font-size: 0.85rem;
  color: var(--texto-terciario);
}

.icone-status {
  font-size: 0.9rem;
}

.texto-status {
  font-weight: 500;
}

/* Grade de perfis */
.grade-perfis {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(450px, 1fr));
  gap: 2rem;
}

/* Cartão de perfil */
.cartao-perfil {
  background: var(--fundo-cartao);
  border-radius: 25px;
  padding: 2rem;
  box-shadow: 0 10px 30px var(--sombra-cartao);
  border: 2px solid var(--cor-perfil);
  transition: all 0.3s ease;
  position: relative;
  overflow: hidden;
}

.cartao-perfil::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 4px;
  background: linear-gradient(90deg, var(--cor-perfil), transparent, var(--cor-perfil));
  animation: brilho-perfil 3s ease-in-out infinite;
}

.cartao-perfil:hover {
  transform: translateY(-5px);
  box-shadow: 0 15px 40px var(--sombra-cartao-hover);
}

/* Cabeçalho do cartão */
.cabecalho-cartao {
  display: flex;
  align-items: flex-start;
  gap: 1.5rem;
  margin-bottom: 1.5rem;
}

/* Container do avatar */
.container-avatar {
  position: relative;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.5rem;
}

.avatar-perfil {
  width: 80px;
  height: 80px;
  border-radius: 50%;
  position: relative;
  overflow: hidden;
  border: 3px solid var(--cor-perfil);
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.2);
  cursor: pointer;
  transition: all 0.3s ease;
}

.avatar-perfil:hover {
  transform: scale(1.05);
}

.imagem-avatar {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: all 0.3s ease;
}

.sobreposicao-avatar {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.7);
  display: flex;
  align-items: center;
  justify-content: center;
  opacity: 0;
  transition: opacity 0.3s ease;
}

.avatar-perfil:hover .sobreposicao-avatar {
  opacity: 1;
}

.input-foto-oculto {
  display: none;
}

.botao-alterar-foto {
  background: none;
  border: none;
  color: white;
  font-size: 1.5rem;
  cursor: pointer;
  transition: transform 0.3s ease;
}

.botao-alterar-foto:hover {
  transform: scale(1.2);
}

.emoji-perfil {
  font-size: 1.5rem;
  background: var(--cor-perfil);
  width: 30px;
  height: 30px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.2);
}

/* Informações básicas */
.informacoes-basicas {
  flex: 1;
}

.nome-perfil {
  font-size: 1.8rem;
  font-weight: 700;
  color: var(--texto-primario);
  margin-bottom: 0.75rem;
}

.detalhes-perfil {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.idade-perfil,
.aniversario-perfil,
.profissao-perfil,
.cidade-perfil {
  color: var(--texto-secundario);
  font-size: 0.9rem;
  font-weight: 500;
  margin: 0;
}

/* Descrição do perfil */
.descricao-perfil {
  margin-bottom: 1.5rem;
  padding: 1rem;
  background: var(--fundo-descricao);
  border-radius: 15px;
  border-left: 4px solid var(--cor-perfil);
}

.descricao-perfil p {
  color: var(--texto-secundario);
  line-height: 1.6;
  font-style: italic;
  margin: 0;
}

/* Seção de hobbies */
.secao-hobbies {
  margin-bottom: 1.5rem;
}

.titulo-hobbies {
  font-size: 1.1rem;
  color: var(--texto-primario);
  margin-bottom: 0.75rem;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.icone-hobbies {
  font-size: 1rem;
}

.lista-hobbies {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
}

.tag-hobby {
  background: var(--cor-perfil);
  color: white;
  padding: 0.25rem 0.75rem;
  border-radius: 15px;
  font-size: 0.8rem;
  font-weight: 500;
}

/* Seção de mensagem */
.secao-mensagem {
  margin-bottom: 2rem;
}

.titulo-mensagem {
  font-size: 1.1rem;
  color: var(--texto-primario);
  margin-bottom: 1rem;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.icone-mensagem {
  font-size: 1rem;
}

.container-input-mensagem {
  position: relative;
  margin-bottom: 1rem;
}

.campo-mensagem {
  width: 100%;
  padding: 1rem;
  border: 2px solid var(--borda-input);
  border-radius: 15px;
  background: var(--fundo-input);
  color: var(--texto-primario);
  font-family: inherit;
  font-size: 0.95rem;
  line-height: 1.5;
  resize: vertical;
  min-height: 80px;
  transition: all 0.3s ease;
}

.campo-mensagem:focus {
  outline: none;
  border-color: var(--cor-perfil);
  box-shadow: 0 0 0 3px rgba(236, 72, 153, 0.1);
}

.contador-caracteres {
  position: absolute;
  bottom: 8px;
  right: 12px;
  font-size: 0.8rem;
  color: var(--texto-terciario);
  background: var(--fundo-contador);
  padding: 2px 6px;
  border-radius: 8px;
}

.botao-enviar-mensagem {
  background: linear-gradient(135deg, var(--cor-perfil), #8B5CF6);
  color: white;
  border: none;
  padding: 0.75rem 1.5rem;
  border-radius: 20px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.9rem;
}

.botao-enviar-mensagem:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 5px 15px rgba(236, 72, 153, 0.3);
}

.botao-enviar-mensagem:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.spinner-pequeno {
  width: 16px;
  height: 16px;
  border: 2px solid rgba(255, 255, 255, 0.3);
  border-top: 2px solid white;
  border-radius: 50%;
  animation: girar 1s linear infinite;
}

.icone-botao {
  font-size: 1rem;
}

/* Histórico de mensagens */
.historico-mensagens {
  border-top: 1px solid var(--borda-secao);
  padding-top: 1.5rem;
}

.titulo-historico {
  font-size: 1.1rem;
  color: var(--texto-primario);
  margin-bottom: 1rem;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  flex-wrap: wrap;
}

.icone-historico {
  font-size: 1rem;
}

.timestamp-atualizacao {
  font-size: 0.8rem;
  color: var(--texto-terciario);
  font-weight: 400;
}

.lista-mensagens {
  max-height: 300px;
  overflow-y: auto;
  padding-right: 0.5rem;
}

.item-mensagem {
  background: var(--fundo-mensagem);
  border-radius: 12px;
  padding: 1rem;
  margin-bottom: 0.75rem;
  border-left: 3px solid var(--cor-perfil);
  transition: all 0.3s ease;
}

.item-mensagem:hover {
  background: var(--fundo-mensagem-hover);
  transform: translateX(5px);
}

.cabecalho-mensagem {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.5rem;
}

.data-mensagem {
  font-weight: 600;
  color: var(--cor-perfil);
  font-size: 0.85rem;
}

.hora-mensagem {
  color: var(--texto-terciario);
  font-size: 0.8rem;
}

.conteudo-mensagem {
  color: var(--texto-secundario);
  line-height: 1.5;
  font-size: 0.9rem;
}

/* Estado vazio */
.sem-mensagens {
  text-align: center;
  padding: 2rem;
  color: var(--texto-terciario);
  border-top: 1px solid var(--borda-secao);
  margin-top: 1.5rem;
}

.icone-sem-mensagens {
  font-size: 2rem;
  margin-bottom: 0.5rem;
  opacity: 0.5;
}

.sem-mensagens p {
  margin-bottom: 0.25rem;
}

.texto-incentivo {
  font-size: 0.85rem;
  color: var(--cor-perfil);
  font-weight: 500;
}

/* Indicador de carregamento */
.indicador-carregamento-foto {
  position: fixed;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  background: var(--fundo-cartao);
  padding: 2rem;
  border-radius: 15px;
  box-shadow: 0 10px 30px var(--sombra-cartao);
  text-align: center;
  z-index: 1000;
}

.spinner-carregamento {
  width: 40px;
  height: 40px;
  border: 4px solid var(--borda-input);
  border-top: 4px solid var(--cor-perfil);
  border-radius: 50%;
  animation: girar 1s linear infinite;
  margin: 0 auto 1rem;
}

/* Animações */
@keyframes brilho-perfil {
  0%, 100% {
    opacity: 0.5;
  }
  50% {
    opacity: 1;
  }
}

@keyframes girar {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

/* Scrollbar personalizada */
.lista-mensagens::-webkit-scrollbar {
  width: 6px;
}

.lista-mensagens::-webkit-scrollbar-track {
  background: var(--fundo-scrollbar);
  border-radius: 3px;
}

.lista-mensagens::-webkit-scrollbar-thumb {
  background: var(--cor-perfil);
  border-radius: 3px;
}

.lista-mensagens::-webkit-scrollbar-thumb:hover {
  background: var(--cor-perfil);
  opacity: 0.8;
}

/* Design responsivo */
@media (max-width: 768px) {
  .secao-perfis {
    padding: 1rem;
  }
  
  .titulo-perfis {
    font-size: 2rem;
  }
  
  .grade-perfis {
    grid-template-columns: 1fr;
    gap: 1.5rem;
  }
  
  .cartao-perfil {
    padding: 1.5rem;
  }
  
  .cabecalho-cartao {
    flex-direction: column;
    text-align: center;
    gap: 1rem;
  }
  
  .container-avatar {
    align-self: center;
  }
  
  .avatar-perfil {
    width: 70px;
    height: 70px;
  }
  
  .nome-perfil {
    font-size: 1.5rem;
  }
  
  .indicador-sincronizacao {
    flex-direction: column;
    gap: 0.75rem;
  }
}

@media (max-width: 480px) {
  .cartao-perfil {
    padding: 1rem;
  }
  
  .avatar-perfil {
    width: 60px;
    height: 60px;
  }
  
  .nome-perfil {
    font-size: 1.3rem;
  }
  
  .lista-hobbies {
    justify-content: center;
  }
}

/* Variáveis CSS para temas */
:root {
  --texto-primario: #374151;
  --texto-secundario: #6B7280;
  --texto-terciario: #9CA3AF;
  --fundo-cartao: rgba(255, 255, 255, 0.8);
  --sombra-cartao: rgba(0, 0, 0, 0.1);
  --sombra-cartao-hover: rgba(0, 0, 0, 0.15);
  --fundo-descricao: rgba(255, 255, 255, 0.5);
  --borda-input: #E5E7EB;
  --fundo-input: rgba(255, 255, 255, 0.8);
  --fundo-contador: rgba(255, 255, 255, 0.9);
  --borda-secao: rgba(0, 0, 0, 0.1);
  --fundo-mensagem: rgba(255, 255, 255, 0.6);
  --fundo-mensagem-hover: rgba(255, 255, 255, 0.8);
  --fundo-scrollbar: rgba(0, 0, 0, 0.1);
  --fundo-sincronizacao: rgba(255, 255, 255, 0.7);
  --borda-sincronizacao: rgba(0, 0, 0, 0.1);
  --texto-sincronizacao: #6B7280;
  --cor-spinner-track: rgba(0, 0, 0, 0.1);
  --cor-primaria: #EC4899;
}

:global(.dark-theme) {
  --texto-primario: #F9FAFB;
  --texto-secundario: #D1D5DB;
  --texto-terciario: #9CA3AF;
  --fundo-cartao: rgba(0, 0, 0, 0.4);
  --sombra-cartao: rgba(0, 0, 0, 0.3);
  --sombra-cartao-hover: rgba(0, 0, 0, 0.4);
  --fundo-descricao: rgba(0, 0, 0, 0.3);
  --borda-input: #374151;
  --fundo-input: rgba(0, 0, 0, 0.3);
  --fundo-contador: rgba(0, 0, 0, 0.5);
  --borda-secao: rgba(255, 255, 255, 0.1);
  --fundo-mensagem: rgba(0, 0, 0, 0.2);
  --fundo-mensagem-hover: rgba(0, 0, 0, 0.3);
  --fundo-scrollbar: rgba(255, 255, 255, 0.1);
  --fundo-sincronizacao: rgba(0, 0, 0, 0.3);
  --borda-sincronizacao: rgba(255, 255, 255, 0.1);
  --texto-sincronizacao: #D1D5DB;
  --cor-spinner-track: rgba(255, 255, 255, 0.1);
  --cor-primaria: #EC4899;
}
</style>