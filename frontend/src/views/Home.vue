<template>
  <v-container>
    <v-card>
      <v-card-title>Главная страница</v-card-title>
      <v-card-text>
        <p>Вы не вошли в систему. Зарегистрируйтесь или войдите, чтобы продолжить.</p>
      </v-card-text>
      <v-card-actions>
        <v-btn color="primary" to="/auth">Перейти на страницу регистрации</v-btn>
      </v-card-actions>
    </v-card>
  </v-container>
</template>

<script lang="ts">
import { defineComponent } from 'vue';

export default defineComponent({
  name: "Home",
});
</script>