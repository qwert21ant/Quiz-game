<template>
  <div class="ma-auto" v-if="!isInitialized">
    <v-progress-circular indeterminate size="x-large"/>
  </div>
  <div v-else>
    <v-container max-width="1000px">
      <v-card>
        <v-card-title class="d-flex">
          <div>Комната {{ roomInfo.name }}</div>
          <v-btn
            class="ml-auto"
            color="error"
            variant="outlined"
            prepend-icon="mdi-close"
            text="Покинуть"
            @click="leaveRoom"
          />
        </v-card-title>
        <v-card-text>
          {{ roomInfo.description ?? "Без описания" }}
        </v-card-text>
      </v-card>
    </v-container>
  </div>
</template>

<script lang="ts">
import RoomPublicInfo from '@/models/RoomPublicInfo';
import router from '@/router';
import RoomParticipantService from '@/services/RoomParticipantService';
import { defineComponent } from 'vue';

export default defineComponent({
  name: "Game",
  data() {
    return {
      roomService: new RoomParticipantService(),
      roomInfo: null as RoomPublicInfo,

      isInitialized: false as boolean,
    };
  },
  methods: {
    async leaveRoom() {
      await this.roomService.leaveRoom({
        id: this.$route.params.id,
      });

      router.push({ path: "/dashboard" });
    },
  },
  async mounted() {
    this.roomInfo = await this.roomService.getInfo({
      id: this.$route.params.id,
    });

    this.isInitialized = true;
  },
});
</script>