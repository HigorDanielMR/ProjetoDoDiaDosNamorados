import axios from 'axios'

const URL_BASE_API = 'http://localhost:5000/api'

const api = axios.create({
  baseURL: URL_BASE_API,
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
  },
})

export interface RespostaContagem {
  anos: number
  meses: number
  dias: number
  horas: number
  minutos: number
  segundos: number
  milissegundos: number
  totalDias: number
  totalHoras: number
  totalMinutos: number
  totalSegundos: number
  totalMilissegundos: number
}

export interface Foto {
  id: string
  nome: string
  nomeArquivo: string
  url: string
  dataEnvio: string
  tamanho: number
}

export interface Perfil {
  id: string
  nome: string
  genero: string
  idade: number
  aniversario: string
  descricao: string
  cor: string
  emoji: string
  fotoPerfil: string
  hobbies: string[]
  profissao: string
  cidadeNatal: string
}

export interface MensagemCarinho {
  id: string
  mensagem: string
  data: string
  dataCompleta: string
  idPerfil: string
}

// API para contagem de tempo
export const apiContagem = {
  obterDataInicial: async () => {
    try {
      const resposta = await api.get('/contagem/data-inicial')
      return resposta.data
    } catch (erro) {
      console.error('Erro ao buscar data inicial:', erro)
      throw erro
    }
  },

  obterTempoAtual: async () => {
    try {
      const resposta = await api.get('/contagem/tempo-atual')
      return resposta.data
    } catch (erro) {
      console.error('Erro ao buscar tempo atual:', erro)
      throw erro
    }
  },

  calcularContagem: async (): Promise<RespostaContagem> => {
    try {
      const resposta = await api.get('/contagem/calcular')
      return resposta.data
    } catch (erro) {
      console.error('Erro ao calcular contagem:', erro)
      throw erro
    }
  },
}

// API para gerenciar fotos
export const apiFotos = {
  enviarFotos: async (arquivos: File[]): Promise<Foto[]> => {
    try {
      const dadosFormulario = new FormData()
      arquivos.forEach(arquivo => {
        dadosFormulario.append('files', arquivo)
      })

      const resposta = await api.post('/fotos/enviar', dadosFormulario, {
        headers: {
          'Content-Type': 'multipart/form-data',
        },
      })

      return resposta.data.fotos.map((foto: any) => ({
        ...foto,
        url: `http://localhost:5000${foto.url}`
      }))
    } catch (erro) {
      console.error('Erro ao fazer upload das fotos:', erro)
      throw erro
    }
  },

  obterTodasFotos: async (): Promise<Foto[]> => {
    try {
      const resposta = await api.get('/fotos')
      return resposta.data.fotos.map((foto: any) => ({
        ...foto,
        url: `http://localhost:5000${foto.url}`
      }))
    } catch (erro) {
      console.error('Erro ao buscar fotos:', erro)
      throw erro
    }
  },

  deletarFoto: async (idFoto: string): Promise<void> => {
    try {
      await api.delete(`/fotos/${idFoto}`)
    } catch (erro) {
      console.error('Erro ao deletar foto:', erro)
      throw erro
    }
  },
}

// API para gerenciar perfis e mensagens
export const apiPerfis = {
  obterPerfis: async (): Promise<Perfil[]> => {
    try {
      const resposta = await api.get('/perfis')
      return resposta.data.perfis
    } catch (erro) {
      console.error('Erro ao buscar perfis:', erro)
      throw erro
    }
  },

  obterMensagensPerfil: async (idPerfil: string): Promise<MensagemCarinho[]> => {
    try {
      const resposta = await api.get(`/mensagens/${idPerfil}`)
      return resposta.data.mensagens
    } catch (erro) {
      console.error('Erro ao buscar mensagens do perfil:', erro)
      throw erro
    }
  },

  salvarMensagemCarinho: async (idPerfil: string, mensagem: string): Promise<MensagemCarinho> => {
    try {
      const resposta = await api.post(`/mensagens/${idPerfil}`, { mensagem })
      return resposta.data.dados
    } catch (erro) {
      console.error('Erro ao salvar mensagem de carinho:', erro)
      throw erro
    }
  },

  // Nova função para upload de foto de perfil
  enviarFotoPerfil: async (idPerfil: string, arquivo: File): Promise<{ urlFoto: string; nomeArquivo: string }> => {
    try {
      const dadosFormulario = new FormData()
      dadosFormulario.append('file', arquivo)

      const resposta = await api.post(`/perfis/${idPerfil}/foto`, dadosFormulario, {
        headers: {
          'Content-Type': 'multipart/form-data',
        },
      })

      return {
        urlFoto: `http://localhost:5000${resposta.data.urlFoto}`,
        nomeArquivo: resposta.data.nomeArquivo
      }
    } catch (erro) {
      console.error('Erro ao fazer upload da foto de perfil:', erro)
      throw erro
    }
  },
}

export default api