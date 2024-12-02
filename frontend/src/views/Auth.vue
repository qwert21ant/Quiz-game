<template>
  <v-container>
    <v-card>
      <v-card-title>
        <v-tabs v-model="tab" background-color="primary">
          <v-tab>Войти</v-tab>
          <v-tab>Зарегистрироваться</v-tab>
        </v-tabs>
      </v-card-title>

      <v-card-text class="mt-3" style="padding-bottom: 0;">
        <v-text-field
          v-model="username"
          class="ml-2"
          label="Username"
          type="email"
          prepend-icon="mdi-account"
          :rules="[usernameRule]"
          :disabled="isLoading"
          @change="error = ''"
        />
        <v-text-field
          v-model="password"
          class="ml-2"
          label="Password"
          type="password"
          prepend-icon="mdi-lock"
          :rules="[passwordRule]"
          :disabled="isLoading"
          @change="error = ''"
        />
      </v-card-text>
      <v-card-actions class="justify-space-between">
        <div v-if="error" class="pl-2 text-error d-flex">
          <v-icon icon="mdi-alert-circle"/>
          <p class="ml-2">{{ error }}</p>
        </div>
        <div class="ml-auto">
          <v-btn
            v-if="tab == 0"
            color="primary"
            @click="login"
            :disabled="!isValid"
            :loading="isLoading"
          >
            Войти
          </v-btn>
          <v-btn
            v-else
            color="primary"
            @click="signup"
            :disabled="!isValid"
            :loading="isLoading"
          >
            Зарегистрироваться
          </v-btn>
        </div>
      </v-card-actions>
    </v-card>
  </v-container>
</template>

<script lang="ts">
import router from '@/router';
import AuthService from '@/services/AuthService';
import { defineComponent } from 'vue';

export default defineComponent({
  data() {
    return {
      authService: new AuthService(),

      username: "" as string,
      password: "" as string,

      isLoading: false as boolean,

      error: "" as string,

      tab: 0 as number,
    };
  },
  computed: {
    isValid(): boolean {
      return this.usernameRule(this.username) === true
        && this.passwordRule(this.password) === true;
    },
  },
  methods: {
    async login() {
      this.isLoading = true;
      
      const res = await this.authService.login({
        username: this.username,
        password: this.password,
      });

      if (!res)
        this.error = "Неверное имя пользователя или пароль";
      else
        router.push({ path: "/dashboard" });

      this.isLoading = false;
    },
    async signup() {
      this.isLoading = true;
      
      const res = await this.authService.signup({
        username: this.username,
        password: this.password,
      });

      if (!res)
        this.error = "Имя пользователя уже занято";
      else
        router.push({ path: "/dashboard" });

      this.isLoading = false;
    },
    usernameRule(input: string) {
      return input.length > 0 ? true : "Это поле не должно быть пустым";
    },
    passwordRule(input: string) {
      return input.length >= 4 ? true : "Минимальная длина пароля: 4";
    }
  },
});
</script>
