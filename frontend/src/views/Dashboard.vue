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
    <v-container style="margin-top: 60px">
      <v-card>
        <v-card-title>Добро пожаловать!</v-card-title>
        <v-card-text>
          <p>Вы вошли в систему.</p>
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
  },
  async mounted() {
    this.userData = await this.userService.getUserData();
  },
});
</script>
