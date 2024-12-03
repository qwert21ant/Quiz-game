<template>
  <div class="ma-auto" v-if="!userData">
    <v-progress-circular indeterminate size="x-large"/>
  </div>
  <div v-else>
    <v-app-bar height="60" elevation="3">
      <v-app-bar-title>Quiz game</v-app-bar-title>
      <template #append>
        <v-btn class="fill-height">
          <v-icon
            class="mr-2"
            icon="mdi-account-circle"
            size="x-large"
          />
          <div>{{ userData.username }}</div>

          <v-menu activator="parent">
            <v-list>
              <v-list-item
                class="text-error"
                @click="logout"
              >
              <v-icon
                class="mr-2"
                icon="mdi-logout"
              />
              <span>LOGOUT</span>
              </v-list-item>
            </v-list>
          </v-menu>
        </v-btn>
      </template>
    </v-app-bar>
    
    <div style="height: 60px"></div>
    <v-container
      v-if="userData.activeRoom"
    >
      <v-card>
        <v-card-title>Добро пожаловать!</v-card-title>
        <v-card-text>
          <p>Вы вошли в систему.</p>
        </v-card-text>
      </v-card>
    </v-container>
    <v-container>
      <v-card>
        <v-card-title class="d-flex justify-space-between">
          <div>
            Мои квизы
          </div>
          <v-btn
            prepend-icon="mdi-plus"
            @click="createQuiz"
          >
            Создать
          </v-btn>
        </v-card-title>
        <v-card-text>
          <div v-if="!userData.quizes.length">
            У вас нет квизов
          </div>
          <div v-else class="d-flex flex-wrap">
            <v-card
              v-for="quiz in userData.quizes"
              class="ma-1"
              width="250"
              @click="openQuiz(quiz.name)"
            >
              <v-card-title>{{ quiz.name }}</v-card-title>
              <v-card-text>{{ quiz.name }}</v-card-text>
            </v-card>
          </div>
        </v-card-text>
      </v-card>
    </v-container>
  </div>
</template>

<script lang="ts">
import UserData from '@/models/UserData';
import router from '@/router';
import AuthService from '@/services/AuthService';
import UserService from '@/services/UserService';
import { defineComponent } from 'vue';

export default defineComponent({
  name: "Dashboard",
  data() {
    return {
      authService: new AuthService(),
      userService: new UserService(),

      userData: null as UserData,
    };
  },
  methods: {
    async logout() {
      await this.authService.logout();
      router.push({ path: "/auth" });
    },
    createQuiz() {

    },
    openQuiz(name: string) {

    },
  },
  async mounted() {
    this.userData = await this.userService.getUserData();
  },
});
</script>
