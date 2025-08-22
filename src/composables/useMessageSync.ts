import { ref, reactive, onUnmounted, readonly } from 'vue'
import { apiPerfis, type MensagemCarinho } from '../services/api'

// Estado global das mensagens
const mensagensGlobais = reactive<Record<string, MensagemCarinho[]>>({})
const ultimaAtualizacao = ref<Record<string, string>>({})
const estaCarregando = ref(false)
const intervalosAtivos = new Set<number>()

// Configurações do sistema de sincronização
const INTERVALO_ATUALIZACAO = 5000 // 5 segundos
const INTERVALO_ATUALIZACAO_RAPIDA = 2000 // 2 segundos após envio
const TEMPO_ATUALIZACAO_RAPIDA = 30000 // 30 segundos de atualização rápida

export const useMessageSync = () => {
  let intervaloSincronizacao: number | null = null
  let timeoutAtualizacaoRapida: number | null = null

  // Função para carregar mensagens de um perfil específico
  const carregarMensagensPerfil = async (idPerfil: string, silencioso = false) => {
    try {
      if (!silencioso) {
        estaCarregando.value = true
      }

      console.log(`🔄 Carregando mensagens do perfil: ${idPerfil}`)
      const mensagens = await apiPerfis.obterMensagensPerfil(idPerfil)
      
      // Verificar se há novas mensagens
      const mensagensAtuais = mensagensGlobais[idPerfil] || []
      const novasMensagens = mensagens.filter(novaMensagem => 
        !mensagensAtuais.some(mensagemExistente => mensagemExistente.id === novaMensagem.id)
      )

      if (novasMensagens.length > 0 && mensagensAtuais.length > 0) {
        console.log(`📨 ${novasMensagens.length} nova(s) mensagem(ns) encontrada(s) para ${idPerfil}`)
      }
      
      // Atualizar estado global
      mensagensGlobais[idPerfil] = mensagens
      ultimaAtualizacao.value[idPerfil] = new Date().toISOString()
      
      console.log(`✅ Mensagens do perfil ${idPerfil} atualizadas:`, mensagens.length, 'mensagens')
      
      return mensagens
    } catch (erro) {
      console.error(`❌ Erro ao carregar mensagens do perfil ${idPerfil}:`, erro)
      return mensagensGlobais[idPerfil] || []
    } finally {
      if (!silencioso) {
        estaCarregando.value = false
      }
    }
  }

  // Função para carregar mensagens de todos os perfis
  const carregarTodasMensagens = async (perfis: string[], silencioso = false) => {
    try {
      if (!silencioso) {
        estaCarregando.value = true
      }

      console.log('🔄 Carregando mensagens de todos os perfis:', perfis)
      const promessas = perfis.map(idPerfil => carregarMensagensPerfil(idPerfil, true))
      await Promise.all(promessas)
      
      console.log('✅ Todas as mensagens foram sincronizadas')
    } catch (erro) {
      console.error('❌ Erro ao carregar todas as mensagens:', erro)
    } finally {
      if (!silencioso) {
        estaCarregando.value = false
      }
    }
  }

  // Função para adicionar nova mensagem ao estado global
  const adicionarMensagemLocal = (idPerfil: string, novaMensagem: MensagemCarinho) => {
    if (!mensagensGlobais[idPerfil]) {
      mensagensGlobais[idPerfil] = []
    }
    
    // Verificar se a mensagem já existe (evitar duplicatas)
    const jaExiste = mensagensGlobais[idPerfil].some(msg => msg.id === novaMensagem.id)
    if (!jaExiste) {
      mensagensGlobais[idPerfil].push(novaMensagem)
      ultimaAtualizacao.value[idPerfil] = new Date().toISOString()
      console.log(`➕ Nova mensagem adicionada localmente para ${idPerfil}:`, novaMensagem.mensagem)
    } else {
      console.log(`⚠️ Mensagem duplicada ignorada para ${idPerfil}`)
    }
  }

  // Função para iniciar sincronização automática
  const iniciarSincronizacao = (perfis: string[]) => {
    console.log('🚀 Iniciando sincronização automática para perfis:', perfis)
    
    if (intervaloSincronizacao) {
      clearInterval(intervaloSincronizacao)
      intervalosAtivos.delete(intervaloSincronizacao)
    }

    // Carregar mensagens inicialmente
    carregarTodasMensagens(perfis, true)

    // Configurar sincronização periódica
    intervaloSincronizacao = window.setInterval(() => {
      console.log('🔄 Executando sincronização automática...')
      carregarTodasMensagens(perfis, true)
    }, INTERVALO_ATUALIZACAO)

    intervalosAtivos.add(intervaloSincronizacao)
    console.log('✅ Sincronização automática iniciada com intervalo de', INTERVALO_ATUALIZACAO, 'ms')
  }

  // Função para ativar atualização rápida após envio de mensagem
  const ativarAtualizacaoRapida = (perfis: string[]) => {
    console.log('⚡ Ativando atualização rápida por 30 segundos')
    
    // Limpar intervalo normal
    if (intervaloSincronizacao) {
      clearInterval(intervaloSincronizacao)
      intervalosAtivos.delete(intervaloSincronizacao)
    }

    // Limpar timeout anterior se existir
    if (timeoutAtualizacaoRapida) {
      clearTimeout(timeoutAtualizacaoRapida)
    }

    // Iniciar atualização rápida
    const intervaloRapido = window.setInterval(() => {
      console.log('⚡ Executando sincronização rápida...')
      carregarTodasMensagens(perfis, true)
    }, INTERVALO_ATUALIZACAO_RAPIDA)

    intervalosAtivos.add(intervaloRapido)
    console.log('⚡ Atualização rápida ativada com intervalo de', INTERVALO_ATUALIZACAO_RAPIDA, 'ms')

    // Voltar ao intervalo normal após 30 segundos
    timeoutAtualizacaoRapida = window.setTimeout(() => {
      console.log('🔄 Voltando ao intervalo normal de sincronização')
      clearInterval(intervaloRapido)
      intervalosAtivos.delete(intervaloRapido)
      iniciarSincronizacao(perfis)
    }, TEMPO_ATUALIZACAO_RAPIDA)
  }

  // Função para parar sincronização
  const pararSincronizacao = () => {
    console.log('⏹️ Parando sincronização...')
    
    if (intervaloSincronizacao) {
      clearInterval(intervaloSincronizacao)
      intervalosAtivos.delete(intervaloSincronizacao)
      intervaloSincronizacao = null
    }

    if (timeoutAtualizacaoRapida) {
      clearTimeout(timeoutAtualizacaoRapida)
      timeoutAtualizacaoRapida = null
    }

    // Limpar todos os intervalos ativos
    intervalosAtivos.forEach(intervalo => {
      clearInterval(intervalo)
    })
    intervalosAtivos.clear()

    console.log('✅ Sincronização parada')
  }

  // Função para obter mensagens de um perfil
  const obterMensagensPerfil = (idPerfil: string): MensagemCarinho[] => {
    const mensagens = mensagensGlobais[idPerfil] || []
    console.log(`📋 Obtendo mensagens do perfil ${idPerfil}:`, mensagens.length, 'mensagens')
    return mensagens
  }

  // Função para verificar se há mensagens para um perfil
  const temMensagens = (idPerfil: string): boolean => {
    const tem = (mensagensGlobais[idPerfil] || []).length > 0
    console.log(`❓ Perfil ${idPerfil} tem mensagens:`, tem)
    return tem
  }

  // Função para obter contagem de mensagens
  const contarMensagens = (idPerfil: string): number => {
    const count = (mensagensGlobais[idPerfil] || []).length
    console.log(`🔢 Contagem de mensagens para ${idPerfil}:`, count)
    return count
  }

  // Função para obter última atualização
  const obterUltimaAtualizacao = (idPerfil: string): string | null => {
    return ultimaAtualizacao.value[idPerfil] || null
  }

  // Cleanup automático quando o componente é desmontado
  onUnmounted(() => {
    pararSincronizacao()
  })

  return {
    // Estado reativo
    mensagensGlobais: readonly(mensagensGlobais),
    ultimaAtualizacao: readonly(ultimaAtualizacao),
    estaCarregando: readonly(estaCarregando),

    // Funções de gerenciamento
    carregarMensagensPerfil,
    carregarTodasMensagens,
    adicionarMensagemLocal,
    iniciarSincronizacao,
    ativarAtualizacaoRapida,
    pararSincronizacao,

    // Funções de consulta
    obterMensagensPerfil,
    temMensagens,
    contarMensagens,
    obterUltimaAtualizacao
  }
}

// Instância global para uso em toda a aplicação
export const messageSyncGlobal = useMessageSync()