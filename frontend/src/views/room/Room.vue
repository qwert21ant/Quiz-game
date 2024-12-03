<template>
  <div class="ma-auto" v-if="!roomConfig">
    <v-progress-circular indeterminate size="x-large"/>
  </div>
  <div v-else>
    <AppBar :username="userData.username" />

    <v-container max-width="1000px">
      <v-card>
        <v-card-title>
          <div class="mb-1 d-flex align-center">
            <v-btn
              class="mr-3"
              icon="mdi-arrow-left"
              variant="text"
              @click="back"
            />
            <div>Настройки комнаты</div>
            <div v-if="false" class="ml-auto d-flex align-center">
              <div>ID: {{ "#123" }}</div>
              <v-btn
                class="ml-3"
                icon="mdi-content-copy"
                size="x-small"
                variant="outlined"
              />
            </div>
          </div>
          <v-divider thickness="2"/>
        </v-card-title>
        <v-card-text style="padding-bottom: 0;">
          <v-text-field
            v-model="roomConfig.name"
            label="Name"
          />
          <v-text-field
            v-model="roomConfig.description"
            label="Description"
          />
          <v-select
            v-model="roomConfig.quizName"
            label="Quiz"
            :items="quizNames"
          />
          <v-text-field
            v-model="roomConfig.maxParticipants"
            type="number"
            label="Max participants"
          />
        </v-card-text>
        <v-card-actions>
          <v-btn
            class="ml-auto"
            variant="flat"
            text="Открыть"
            @click="openRoom"
          />
        </v-card-actions>
      </v-card>
    </v-container>
  </div>
</template>

<script lang="ts">
import RoomConfig from '@/models/RoomConfig';
import UserData from '@/models/UserData';
import RoomService from '@/services/RoomService';
import UserService from '@/services/UserService';
import { defineComponent } from 'vue';
import AppBar from '../common/AppBar.vue';
import router from '@/router';

export default defineComponent({
  name: "Room",
  components: { AppBar },
  data() {
    return {
      userService: new UserService(),
      roomService: new RoomService(),

      userData: null as UserData,
      roomConfig: null as RoomConfig,

      roomName: "" as string,
      roomDescription: "" as string,
      quizName: "" as string,
      maxParticipants: 0 as number,
    };
  },
  computed: {
    quizNames() {
      return this.userData.quizes.map(q => q.name);
    },
  },
  methods: {
    back() {
      router.push({ path: "/dashboard" });
    },
    async openRoom() {
      this.roomConfig.maxParticipants = Number(this.roomConfig.maxParticipants);

      await this.roomService.updateConfig(this.roomConfig);
    },
  },
  async mounted() {
    this.userData = await this.userService.getUserData();
    this.roomConfig = await this.roomService.getConfig();

    this.roomName = this.roomConfig.name;
    this.roomDescription = this.roomConfig.description;
    this.quizName = this.roomConfig.quizName;
    this.maxParticipants = this.roomConfig.maxParticipants;
  },
});
</script>