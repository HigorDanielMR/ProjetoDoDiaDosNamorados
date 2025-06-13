import { createRouter, createWebHistory } from 'vue-router'
import HomePage from '../views/HomePage.vue'
import CountdownPage from '../views/CountdownPage.vue'

const routes = [
  {
    path: '/',
    name: 'home',
    component: HomePage
  },
  {
    path: '/countdown',
    name: 'countdown',
    component: CountdownPage
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router